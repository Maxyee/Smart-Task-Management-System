// src/app/features/chat/chat.component.ts
import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../core/services/chat.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { Conversation, Message, MessageType, ConversationType, CreateConversationRequest, SendMessageRequest } from '../../core/models/chat.model';
import { User } from '../../core/models/user.model';
import { Subject, takeUntil, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import * as signalR from '@microsoft/signalr';

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
    isConnectionReady = false;

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
        private notificationService: NotificationService,
        private cdr: ChangeDetectorRef // Add this for change detection
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
                const currentUser = this.authService.getCurrentUser();
                this.searchResults = users.filter(u => u.id !== currentUser?.id);
                this.isSearching = false;
                this.cdr.detectChanges();
            },
            error: (error) => {
                console.error('Error searching users:', error);
                this.isSearching = false;
                this.notificationService.error('Failed to search users');
                this.cdr.detectChanges();
            }
        });
    }

    ngOnInit(): void {
        // Subscribe to connection state
        this.chatService.connectionState$
            .pipe(takeUntil(this.destroy$))
            .subscribe(state => {
                this.isConnectionReady = state === signalR.HubConnectionState.Connected;
                if (this.isConnectionReady && this.isChatOpen) {
                    this.loadConversations();
                }
                this.cdr.detectChanges();
            });

        // Load conversations
        this.chatService.conversations$
            .pipe(takeUntil(this.destroy$))
            .subscribe(conversations => {
                this.conversations = conversations;
                this.updateUnreadBadge();
                
                // If there's a selected conversation, update it from the list
                if (this.selectedConversation) {
                    const updated = conversations.find(c => c.id === this.selectedConversation!.id);
                    if (updated) {
                        this.selectedConversation = updated;
                    } else {
                        // If selected conversation is no longer in the list, clear it
                        this.selectedConversation = null;
                        this.showConversationList = true;
                    }
                }
                this.cdr.detectChanges();
            });

        // Load messages
        this.chatService.messages$
            .pipe(takeUntil(this.destroy$))
            .subscribe(messages => {
                if (this.selectedConversation) {
                    this.messages = messages[this.selectedConversation.id] || [];
                    this.scrollToBottom();
                }
                this.cdr.detectChanges();
            });

        // Typing indicators
        this.chatService.typingUsers$
            .pipe(takeUntil(this.destroy$))
            .subscribe(typingMap => {
                if (this.selectedConversation) {
                    this.typingUsers = typingMap[this.selectedConversation.id] || [];
                }
                this.cdr.detectChanges();
            });

        // Online users
        this.chatService.onlineUsers$
            .pipe(takeUntil(this.destroy$))
            .subscribe(users => {
                this.onlineUsers = users;
                this.cdr.detectChanges();
            });

        // Unread count
        this.chatService.unreadCount$
            .pipe(takeUntil(this.destroy$))
            .subscribe(count => {
                this.unreadCount = count;
                this.cdr.detectChanges();
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
            this.cdr.detectChanges();
        }, 1000);
    }

    /**
     * Select a conversation and load its messages
     */
    async selectConversation(conversation: Conversation): Promise<void> {
        if (!conversation) {
            console.warn('No conversation selected');
            return;
        }

        console.log('Selecting conversation:', conversation.id, conversation.name);
        
        // Set the selected conversation
        this.selectedConversation = conversation;
        this.messages = [];
        this.isLoading = true;
        
        // IMPORTANT: Hide the conversation list to show the chat window
        this.showConversationList = false;
        
        // Force change detection
        this.cdr.detectChanges();

        // Check if conversation exists in our list
        const existingConv = this.conversations.find(c => c.id === conversation.id);
        if (!existingConv) {
            // If not in list, add it
            this.conversations = [...this.conversations, conversation];
            this.cdr.detectChanges();
        }

        // Join conversation via SignalR
        if (this.isConnectionReady) {
            try {
                await this.chatService.joinConversation(conversation.id);
                console.log('Joined conversation:', conversation.id);
            } catch (error) {
                console.error('Failed to join conversation:', error);
            }
        }

        // Load messages
        this.chatService.getMessages(conversation.id).subscribe({
            next: (messages) => {
                this.messages = messages || [];
                this.isLoading = false;
                this.scrollToBottom();
                console.log('Loaded messages:', messages.length);

                // Mark as read if there are messages
                if (messages && messages.length > 0) {
                    this.chatService.markAsRead(conversation.id, messages[messages.length - 1].id);
                }

                this.cdr.detectChanges();
            },
            error: (error) => {
                console.error('Error loading messages:', error);
                this.isLoading = false;
                this.messages = [];
                this.cdr.detectChanges();
                // Don't show error for new conversations with no messages
                if (error.message?.includes('404')) {
                    console.log('New conversation - no messages yet');
                } else {
                    this.notificationService.error('Failed to load messages');
                }
            }
        });

        // Focus input after a delay
        setTimeout(() => {
            if (this.messageInput?.nativeElement) {
                this.messageInput.nativeElement.focus();
            }
            this.cdr.detectChanges();
        }, 500);
    }

    /**
     * Send a new message
     */
    async sendMessage(): Promise<void> {
        if (!this.newMessage.trim() || !this.selectedConversation) {
            console.warn('Cannot send message: No content or no conversation selected');
            return;
        }

        const message: SendMessageRequest = {
            conversationId: this.selectedConversation.id,
            content: this.newMessage.trim(),
            type: MessageType.Text
        };

        console.log('Sending message:', message);

        try {
            await this.chatService.sendMessage(message);
            this.newMessage = '';
            this.isTyping = false;

            // Clear typing indicator
            if (this.typingTimeout) {
                clearTimeout(this.typingTimeout);
            }
            await this.chatService.sendTypingIndicator(this.selectedConversation.id, false);
            
            this.cdr.detectChanges();
        } catch (error) {
            console.error('Failed to send message:', error);
            this.notificationService.error('Failed to send message');
        }
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
            this.cdr.detectChanges();
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
            // Reset to show conversation list
            this.showConversationList = true;
            this.selectedConversation = null;
            this.messages = [];
            
            // Check if connection is ready before loading
            if (this.isConnectionReady) {
                this.loadConversations();
            } else {
                setTimeout(() => {
                    this.loadConversations();
                }, 1000);
            }
        }
        this.cdr.detectChanges();
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
            setTimeout(() => {
                this.userSearchInput?.nativeElement?.focus();
            }, 100);
        }
        this.cdr.detectChanges();
    }

    /**
     * Cancel user search
     */
    cancelUserSearch(): void {
        this.isSearchingUsers = false;
        this.userSearchTerm = '';
        this.searchResults = [];
        this.isSearching = false;
        this.cdr.detectChanges();
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
    async startDirectConversation(userId: string): Promise<void> {
        if (this.isCreatingConversation) return;
        
        this.isCreatingConversation = true;
        console.log('Starting conversation with user:', userId);

        // First, check if a conversation already exists with this user
        const existingConversation = this.conversations.find(c => 
            c.type === ConversationType.Direct && 
            c.participants.some(p => p.userId === userId)
        );

        if (existingConversation) {
            console.log('Existing conversation found:', existingConversation.id);
            this.isCreatingConversation = false;
            this.isSearchingUsers = false;
            this.userSearchTerm = '';
            this.searchResults = [];
            await this.selectConversation(existingConversation);
            this.cdr.detectChanges();
            return;
        }

        const request: CreateConversationRequest = {
            type: ConversationType.Direct,
            name: undefined,
            participantIds: [userId]
        };

        console.log('Creating new conversation:', request);

        this.chatService.createConversation(request).subscribe({
            next: async (conversation) => {
                console.log('Conversation created:', conversation);
                this.notificationService.success('Conversation started successfully');
                this.isCreatingConversation = false;
                this.isSearchingUsers = false;
                this.userSearchTerm = '';
                this.searchResults = [];
                
                // Add to conversations list
                this.conversations = [...this.conversations, conversation];
                this.cdr.detectChanges();
                
                // Select the new conversation
                setTimeout(async () => {
                    await this.selectConversation(conversation);
                    this.cdr.detectChanges();
                }, 300);
            },
            error: (error) => {
                console.error('Error creating conversation:', error);
                this.notificationService.error('Failed to start conversation');
                this.isCreatingConversation = false;
                this.cdr.detectChanges();
            }
        });
    }

    /**
     * Handle conversation click from the list
     */
    onConversationClick(conversation: Conversation): void {
        console.log('Conversation clicked:', conversation.id, conversation.name);
        this.selectConversation(conversation);
    }

    /**
     * Check if the view is mobile
     */
    private checkMobileView(): void {
        this.isMobileView = window.innerWidth < 768;
        if (!this.isMobileView) {
            this.showConversationList = true;
        }
        this.cdr.detectChanges();
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
        this.cdr.detectChanges();
    }

    // Helper for template
    Math = Math;
    Object = Object;
}