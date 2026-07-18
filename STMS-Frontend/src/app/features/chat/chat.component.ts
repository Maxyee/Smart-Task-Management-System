// src/app/features/chat/chat.component.ts
import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../core/services/chat.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { Conversation, Message, MessageType, ConversationType, CreateConversationRequest } from '../../core/models/chat.model';
import { User } from '../../core/models/user.model';
import { Subject, takeUntil, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';

@Component({
    selector: 'app-chat',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './chat.component.html',
    styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
    @ViewChild('messageContainer') messageContainer!: ElementRef;
    @ViewChild('messageInput') messageInput!: ElementRef;
    @ViewChild('userSearchInput') userSearchInput!: ElementRef;

    conversations: Conversation[] = [];
    selectedConversation: Conversation | null = null;
    messages: Message[] = [];
    newMessage = '';
    isLoading = false;
    isTyping = false;
    typingUsers: string[] = [];
    onlineUsers: string[] = [];
    unreadCount = 0;

    // UI State
    isChatOpen = false;
    isMobileView = false;
    showConversationList = true;

    // User Search State
    isSearchingUsers = false;
    userSearchTerm = '';
    searchResults: User[] = [];
    isSearching = false;
    isCreatingConversation = false;

    private destroy$ = new Subject<void>();
    private typingTimeout: any;
    private searchSubject = new Subject<string>();

    constructor(
        private chatService: ChatService,
        private authService: AuthService,
        private notificationService: NotificationService
    ) {
        // Setup user search with debounce
        this.searchSubject.pipe(
            debounceTime(300),
            distinctUntilChanged(),
            switchMap(searchTerm => {
                if (searchTerm.length >= 2) {
                    this.isSearching = true;
                    return this.chatService.searchUsers(searchTerm);
                }
                this.searchResults = [];
                this.isSearching = false;
                return [];
            }),
            takeUntil(this.destroy$)
        ).subscribe({
            next: (users) => {
                // Filter out current user
                const currentUser = this.authService.getCurrentUser();
                this.searchResults = users.filter(u => u.id !== currentUser?.id);
                this.isSearching = false;
            },
            error: (error) => {
                console.error('Error searching users:', error);
                this.isSearching = false;
                this.notificationService.error('Failed to search users');
            }
        });
    }

    ngOnInit(): void {
        // Load conversations
        this.chatService.conversations$
            .pipe(takeUntil(this.destroy$))
            .subscribe(conversations => {
                this.conversations = conversations;
                this.updateUnreadBadge();
            });

        // Load messages
        this.chatService.messages$
            .pipe(takeUntil(this.destroy$))
            .subscribe(messages => {
                if (this.selectedConversation) {
                    this.messages = messages[this.selectedConversation.id] || [];
                    this.scrollToBottom();
                }
            });

        // Typing indicators
        this.chatService.typingUsers$
            .pipe(takeUntil(this.destroy$))
            .subscribe(typingMap => {
                if (this.selectedConversation) {
                    this.typingUsers = typingMap[this.selectedConversation.id] || [];
                }
            });

        // Online users
        this.chatService.onlineUsers$
            .pipe(takeUntil(this.destroy$))
            .subscribe(users => {
                this.onlineUsers = users;
            });

        // Unread count
        this.chatService.unreadCount$
            .pipe(takeUntil(this.destroy$))
            .subscribe(count => {
                this.unreadCount = count;
            });

        // Check mobile view
        this.checkMobileView();
        window.addEventListener('resize', () => this.checkMobileView());

        if (this.isChatOpen) {
            this.loadConversations();
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
        if (this.typingTimeout) {
            clearTimeout(this.typingTimeout);
        }
    }

    ngAfterViewChecked(): void {
        this.scrollToBottom();
    }

    /**
     * Load conversations from the server
     */
    loadConversations(): void {
        this.isLoading = true;
        this.chatService.loadConversations();
        setTimeout(() => {
            this.isLoading = false;
        }, 1000);
    }

    /**
     * Select a conversation and load its messages
     */
    selectConversation(conversation: Conversation): void {
        this.selectedConversation = conversation;
        this.messages = [];
        this.isLoading = true;

        // Join conversation
        this.chatService.joinConversation(conversation.id);

        // Load messages
        this.chatService.getMessages(conversation.id).subscribe({
            next: (messages) => {
                this.messages = messages;
                this.isLoading = false;
                this.scrollToBottom();

                // Mark as read
                if (messages.length > 0) {
                    this.chatService.markAsRead(conversation.id, messages[messages.length - 1].id);
                }

                // On mobile, hide conversation list
                if (this.isMobileView) {
                    this.showConversationList = false;
                }
            },
            error: (error) => {
                this.isLoading = false;
                this.notificationService.error('Failed to load messages');
                console.error('Error loading messages:', error);
            }
        });

        // Focus input
        setTimeout(() => {
            this.messageInput?.nativeElement?.focus();
        }, 100);
    }

    /**
     * Send a new message
     */
    sendMessage(): void {
        if (!this.newMessage.trim() || !this.selectedConversation) return;

        const message: any = {
            conversationId: this.selectedConversation.id,
            content: this.newMessage.trim(),
            type: MessageType.Text
        };

        this.chatService.sendMessage(message);
        this.newMessage = '';
        this.isTyping = false;

        // Clear typing indicator
        if (this.typingTimeout) {
            clearTimeout(this.typingTimeout);
        }
        this.chatService.sendTypingIndicator(this.selectedConversation.id, false);
    }

    /**
     * Handle typing events
     */
    onTyping(): void {
        if (!this.selectedConversation) return;

        if (!this.isTyping) {
            this.isTyping = true;
            this.chatService.sendTypingIndicator(this.selectedConversation.id, true);
        }

        // Clear previous timeout
        if (this.typingTimeout) {
            clearTimeout(this.typingTimeout);
        }

        // Set timeout to stop typing indicator after 2 seconds of inactivity
        this.typingTimeout = setTimeout(() => {
            this.isTyping = false;
            if (this.selectedConversation) {
                this.chatService.sendTypingIndicator(this.selectedConversation.id, false);
            }
        }, 2000);
    }

    /**
     * Handle key press in message input
     */
    handleKeyPress(event: KeyboardEvent): void {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault();
            this.sendMessage();
        }
    }

    /**
     * Scroll to bottom of message container
     */
    scrollToBottom(): void {
        try {
            const container = this.messageContainer?.nativeElement;
            if (container) {
                container.scrollTop = container.scrollHeight;
            }
        } catch (err) {
            // Ignore
        }
    }

    /**
     * Toggle chat open/closed
     */
    toggleChat(): void {
        this.isChatOpen = !this.isChatOpen;
        if (this.isChatOpen) {
            this.loadConversations();
        }
    }

    /**
     * Toggle user search mode
     */
    toggleUserSearch(): void {
        this.isSearchingUsers = !this.isSearchingUsers;
        this.userSearchTerm = '';
        this.searchResults = [];
        this.isSearching = false;

        if (this.isSearchingUsers) {
            // Focus on search input after render
            setTimeout(() => {
                this.userSearchInput?.nativeElement?.focus();
            }, 100);
        }
    }

    /**
     * Cancel user search
     */
    cancelUserSearch(): void {
        this.isSearchingUsers = false;
        this.userSearchTerm = '';
        this.searchResults = [];
        this.isSearching = false;
    }

    /**
     * Search for users
     */
    searchUsers(event: Event): void {
        const value = (event.target as HTMLInputElement).value;
        this.userSearchTerm = value;
        this.searchSubject.next(value);
    }

    /**
     * Start a direct conversation with a user
     */
    startDirectConversation(userId: string): void {
        this.isCreatingConversation = true;

        const request: CreateConversationRequest = {
            type: ConversationType.Direct,
            name: undefined,
            participantIds: [userId]
        };

        this.chatService.createConversation(request).subscribe({
            next: (conversation) => {
                this.notificationService.success('Conversation started successfully');
                this.isCreatingConversation = false;
                this.isSearchingUsers = false;
                this.userSearchTerm = '';
                this.searchResults = [];
                this.loadConversations();

                // Select the new conversation
                setTimeout(() => {
                    this.selectConversation(conversation);
                }, 500);
            },
            error: (error) => {
                console.error('Error creating conversation:', error);
                this.notificationService.error('Failed to start conversation');
                this.isCreatingConversation = false;
            }
        });
    }

    /**
     * Check if the view is mobile
     */
    private checkMobileView(): void {
        this.isMobileView = window.innerWidth < 768;
        if (!this.isMobileView) {
            this.showConversationList = true;
        }
    }

    /**
     * Update the unread badge count
     */
    private updateUnreadBadge(): void {
        const totalUnread = this.conversations.reduce((sum, conv) => sum + conv.unreadCount, 0);
        document.title = totalUnread > 0
            ? `(${totalUnread}) Smart Task Management`
            : 'Smart Task Management';
        this.unreadCount = totalUnread;
    }

    /**
     * Get typing indicator text
     */
    getTypingText(): string {
        if (this.typingUsers.length === 0) return '';
        if (this.typingUsers.length === 1) return `${this.typingUsers[0]} is typing...`;
        if (this.typingUsers.length === 2) return `${this.typingUsers[0]} and ${this.typingUsers[1]} are typing...`;
        return `${this.typingUsers.length} people are typing...`;
    }

    /**
     * Get initials from name
     */
    getInitials(name: string): string {
        if (!name) return '?';
        return name
            .split(' ')
            .map(n => n[0])
            .join('')
            .toUpperCase()
            .slice(0, 2);
    }

    /**
     * Format timestamp
     */
    formatTime(date: Date): string {
        if (!date) return '';
        const d = new Date(date);
        const now = new Date();

        if (d.toDateString() === now.toDateString()) {
            return d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        }

        if (d.getFullYear() === now.getFullYear()) {
            return d.toLocaleDateString([], { month: 'short', day: 'numeric' });
        }

        return d.toLocaleDateString([], { month: 'short', day: 'numeric', year: 'numeric' });
    }

    /**
     * Check if message is from current user
     */
    isMessageFromCurrentUser(message: Message): boolean {
        const currentUser = this.authService.getCurrentUser();
        return currentUser ? message.senderId === currentUser.id : false;
    }

    /**
     * Get sender name for display
     */
    getSenderName(message: Message): string {
        return this.isMessageFromCurrentUser(message) ? 'You' : message.senderName;
    }

    /**
     * Go back to conversation list (mobile)
     */
    backToList(): void {
        this.showConversationList = true;
        if (this.selectedConversation) {
            this.chatService.leaveConversation(this.selectedConversation.id);
            this.selectedConversation = null;
            this.messages = [];
        }
    }

    // Helper for template
    Math = Math;
    Object = Object;
}