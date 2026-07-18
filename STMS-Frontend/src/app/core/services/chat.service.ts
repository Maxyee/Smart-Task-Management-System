// src/app/core/services/chat.service.ts
import { Injectable, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { ApiService } from './api.service';
import * as signalR from '@microsoft/signalr';
import {
    Conversation,
    Message,
    SendMessageRequest,
    CreateConversationRequest,
    TypingIndicator,
    UserPresence,
    ConversationType,
    MessageType
} from '../models/chat.model';
import { User } from '../models/user.model';

@Injectable({
    providedIn: 'root'
})
export class ChatService implements OnDestroy {
    private hubConnection: signalR.HubConnection;
    private connectionState = new BehaviorSubject<signalR.HubConnectionState>(signalR.HubConnectionState.Disconnected);
    public connectionState$ = this.connectionState.asObservable();

    // Subjects for real-time updates
    private conversationsSubject = new BehaviorSubject<Conversation[]>([]);
    private messagesSubject = new BehaviorSubject<{ [conversationId: string]: Message[] }>({});
    private typingUsersSubject = new BehaviorSubject<{ [conversationId: string]: string[] }>({});
    private onlineUsersSubject = new BehaviorSubject<string[]>([]);
    private unreadCountSubject = new BehaviorSubject<number>(0);

    public conversations$ = this.conversationsSubject.asObservable();
    public messages$ = this.messagesSubject.asObservable();
    public typingUsers$ = this.typingUsersSubject.asObservable();
    public onlineUsers$ = this.onlineUsersSubject.asObservable();
    public unreadCount$ = this.unreadCountSubject.asObservable();

    constructor(
        private apiService: ApiService,
        private http: HttpClient
    ) {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:5027/chathub', {
                accessTokenFactory: () => localStorage.getItem('access_token') || ''
            })
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: retryContext => {
                    // Exponential backoff: 2s, 4s, 8s, 16s, 32s, then stay at 32s
                    const delay = Math.min(32000, Math.pow(2, retryContext.previousRetryCount + 1) * 1000);
                    return delay;
                }
            })
            .build();

        // Track connection state
        this.hubConnection.onreconnecting(() => {
            this.connectionState.next(signalR.HubConnectionState.Reconnecting);
            console.log('SignalR reconnecting...');
        });

        this.hubConnection.onreconnected(() => {
            this.connectionState.next(signalR.HubConnectionState.Connected);
            console.log('SignalR reconnected');
            // Reload conversations after reconnection
            this.loadConversations();
        });

        this.hubConnection.onclose(() => {
            this.connectionState.next(signalR.HubConnectionState.Disconnected);
            console.log('SignalR disconnected');
        });

        this.startConnection();
        this.registerHubEvents();
    }

    ngOnDestroy(): void {
        this.hubConnection.stop();
    }

    private startConnection(): void {
        this.connectionState.next(signalR.HubConnectionState.Connecting);
        
        this.hubConnection
            .start()
            .then(() => {
                this.connectionState.next(signalR.HubConnectionState.Connected);
                console.log('SignalR connection established');
                // Load conversations after connection is established
                setTimeout(() => {
                    this.loadConversations();
                }, 500);
            })
            .catch(err => {
                this.connectionState.next(signalR.HubConnectionState.Disconnected);
                console.error('SignalR connection failed:', err);
                // Retry after 5 seconds
                setTimeout(() => this.startConnection(), 5000);
            });
    }

    /**
     * Register all SignalR hub events
     */
    private registerHubEvents(): void {
        // Receive new message
        this.hubConnection.on('ReceiveMessage', (message: Message) => {
            this.handleNewMessage(message);
        });

        // User online status
        this.hubConnection.on('UserOnline', (userId: string) => {
            this.handleUserOnline(userId);
        });

        this.hubConnection.on('UserOffline', (userId: string) => {
            this.handleUserOffline(userId);
        });

        // Typing indicator
        this.hubConnection.on('UserTyping', (userId: string, isTyping: boolean) => {
            this.handleTypingIndicator(userId, isTyping);
        });

        // Messages read
        this.hubConnection.on('MessagesRead', (conversationId: string, userId: string) => {
            this.handleMessagesRead(conversationId, userId);
        });

        // Message deleted
        this.hubConnection.on('MessageDeleted', (messageId: string) => {
            this.handleMessageDeleted(messageId);
        });

        // Message edited
        this.hubConnection.on('MessageEdited', (message: Message) => {
            this.handleMessageEdited(message);
        });

        // Conversation updated
        this.hubConnection.on('ConversationUpdated', (conversationId: string) => {
            this.loadConversations();
        });

        // New conversation created
        this.hubConnection.on('NewConversation', (conversation: Conversation) => {
            this.loadConversations();
        });

        // Conversations loaded
        this.hubConnection.on('ConversationsLoaded', (conversations: Conversation[]) => {
            this.conversationsSubject.next(conversations);
            this.updateUnreadCount(conversations);
        });

        // Messages loaded
        this.hubConnection.on('MessagesLoaded', (conversationId: string, messages: Message[]) => {
            const currentMessages = this.messagesSubject.value;
            currentMessages[conversationId] = messages;
            this.messagesSubject.next(currentMessages);
        });

        // Error handling
        this.hubConnection.on('Error', (error: string) => {
            console.error('SignalR error:', error);
        });

        // Online users list
        this.hubConnection.on('OnlineUsers', (users: string[]) => {
            this.onlineUsersSubject.next(users);
        });
    }

    /**
     * Check if the connection is ready before invoking
     */
    private isConnectionReady(): boolean {
        return this.hubConnection.state === signalR.HubConnectionState.Connected;
    }

    /**
     * Wait for connection to be ready
     */
    private async waitForConnection(timeoutMs: number = 5000): Promise<boolean> {
        if (this.isConnectionReady()) {
            return true;
        }

        return new Promise((resolve) => {
            const startTime = Date.now();
            const checkInterval = setInterval(() => {
                if (this.isConnectionReady()) {
                    clearInterval(checkInterval);
                    resolve(true);
                } else if (Date.now() - startTime > timeoutMs) {
                    clearInterval(checkInterval);
                    resolve(false);
                }
            }, 200);
        });
    }

    // API Methods
    async loadConversations(): Promise<void> {
        // Wait for connection to be ready
        const isReady = await this.waitForConnection();
        
        if (!isReady) {
            console.warn('SignalR connection not ready, falling back to HTTP');
            // Fallback to HTTP
            this.getConversations().subscribe({
                next: (conversations) => {
                    this.conversationsSubject.next(conversations);
                    this.updateUnreadCount(conversations);
                },
                error: (err) => console.error('Failed to load conversations via HTTP:', err)
            });
            return;
        }

        this.hubConnection.invoke('GetConversations')
            .catch(err => {
                console.error('Failed to load conversations via SignalR:', err);
                // Fallback to HTTP
                this.getConversations().subscribe({
                    next: (conversations) => {
                        this.conversationsSubject.next(conversations);
                        this.updateUnreadCount(conversations);
                    },
                    error: (err) => console.error('Failed to load conversations via HTTP:', err)
                });
            });
    }

    getConversations(): Observable<Conversation[]> {
        return this.apiService.get<Conversation[]>('chat/conversations');
    }

    createConversation(request: CreateConversationRequest): Observable<Conversation> {
        return this.apiService.post<Conversation>('chat/conversations', request);
    }

    getMessages(conversationId: string, page: number = 1, pageSize: number = 50): Observable<Message[]> {
        return this.apiService.get<Message[]>(`chat/conversations/${conversationId}/messages`, {
            page,
            pageSize
        });
    }

    async sendMessage(request: SendMessageRequest): Promise<void> {
        const isReady = await this.waitForConnection();
        
        if (!isReady) {
            console.error('SignalR connection not ready, cannot send message');
            throw new Error('Connection not ready');
        }

        this.hubConnection.invoke('SendMessage', request)
            .catch(err => console.error('Failed to send message:', err));
    }

    async joinConversation(conversationId: string): Promise<void> {
        const isReady = await this.waitForConnection();
        
        if (!isReady) {
            console.warn('SignalR connection not ready, cannot join conversation');
            return;
        }

        this.hubConnection.invoke('JoinConversation', conversationId)
            .catch(err => console.error('Failed to join conversation:', err));
    }

    async leaveConversation(conversationId: string): Promise<void> {
        const isReady = await this.waitForConnection();
        
        if (!isReady) {
            console.warn('SignalR connection not ready, cannot leave conversation');
            return;
        }

        this.hubConnection.invoke('LeaveConversation', conversationId)
            .catch(err => console.error('Failed to leave conversation:', err));
    }

    async markAsRead(conversationId: string, messageId: string): Promise<void> {
        const isReady = await this.waitForConnection();
        
        if (!isReady) {
            console.warn('SignalR connection not ready, cannot mark as read');
            return;
        }

        this.hubConnection.invoke('MarkAsRead', { conversationId, messageId })
            .catch(err => console.error('Failed to mark as read:', err));
    }

    async sendTypingIndicator(conversationId: string, isTyping: boolean): Promise<void> {
        const isReady = await this.waitForConnection();
        
        if (!isReady) {
            console.warn('SignalR connection not ready, cannot send typing indicator');
            return;
        }

        this.hubConnection.invoke('TypingIndicator', { conversationId, isTyping })
            .catch(err => console.error('Failed to send typing indicator:', err));
    }

    async deleteMessage(messageId: string): Promise<void> {
        const isReady = await this.waitForConnection();
        
        if (!isReady) {
            console.warn('SignalR connection not ready, cannot delete message');
            return;
        }

        this.hubConnection.invoke('DeleteMessage', messageId)
            .catch(err => console.error('Failed to delete message:', err));
    }

    async editMessage(messageId: string, newContent: string): Promise<void> {
        const isReady = await this.waitForConnection();
        
        if (!isReady) {
            console.warn('SignalR connection not ready, cannot edit message');
            return;
        }

        this.hubConnection.invoke('EditMessage', messageId, newContent)
            .catch(err => console.error('Failed to edit message:', err));
    }

    /**
     * Search for users to start a new conversation
     */
    searchUsers(searchTerm: string): Observable<User[]> {
        return this.apiService.get<User[]>('users/search', { searchTerm });
    }

    /**
     * Get online users
     */
    async getOnlineUsers(): Promise<void> {
        const isReady = await this.waitForConnection();
        
        if (!isReady) {
            console.warn('SignalR connection not ready, cannot get online users');
            return;
        }

        this.hubConnection.invoke('GetOnlineUsers')
            .catch(err => console.error('Failed to get online users:', err));
    }

    /**
     * Get user presence
     */
    getUserPresence(userId: string): Observable<UserPresence> {
        return this.apiService.get<UserPresence>(`chat/users/${userId}/presence`);
    }

    /**
     * Add participant to conversation
     */
    addParticipant(conversationId: string, userId: string): Observable<void> {
        return this.apiService.post<void>(`chat/conversations/${conversationId}/participants`, { userId });
    }

    /**
     * Remove participant from conversation
     */
    removeParticipant(conversationId: string, userId: string): Observable<void> {
        return this.apiService.delete<void>(`chat/conversations/${conversationId}/participants/${userId}`);
    }

    /**
     * Leave conversation
     */
    leaveConversationApi(conversationId: string): Observable<void> {
        return this.apiService.post<void>(`chat/conversations/${conversationId}/leave`, {});
    }

    // Event Handlers
    private handleNewMessage(message: Message): void {
        // Update messages
        const currentMessages = this.messagesSubject.value;
        const conversationMessages = currentMessages[message.conversationId] || [];
        currentMessages[message.conversationId] = [...conversationMessages, message];
        this.messagesSubject.next(currentMessages);

        // Update conversations list
        this.loadConversations();
    }

    private handleUserOnline(userId: string): void {
        const onlineUsers = this.onlineUsersSubject.value;
        if (!onlineUsers.includes(userId)) {
            this.onlineUsersSubject.next([...onlineUsers, userId]);
        }
    }

    private handleUserOffline(userId: string): void {
        const onlineUsers = this.onlineUsersSubject.value;
        this.onlineUsersSubject.next(onlineUsers.filter(id => id !== userId));
    }

    private handleTypingIndicator(userId: string, isTyping: boolean): void {
        // We need to know which conversation the user is typing in
        // For now, we'll just log it
        console.log(`User ${userId} typing: ${isTyping}`);
    }

    private handleMessagesRead(conversationId: string, userId: string): void {
        // Update messages read status
        const currentMessages = this.messagesSubject.value;
        const conversationMessages = currentMessages[conversationId] || [];
        const updatedMessages = conversationMessages.map(msg => {
            if (msg.senderId !== userId) {
                return { ...msg, isRead: true };
            }
            return msg;
        });
        currentMessages[conversationId] = updatedMessages;
        this.messagesSubject.next(currentMessages);

        // Update unread count
        this.loadConversations();
    }

    private handleMessageDeleted(messageId: string): void {
        // Remove message from state
        const currentMessages = this.messagesSubject.value;
        for (const conversationId in currentMessages) {
            const messages = currentMessages[conversationId];
            const index = messages.findIndex(m => m.id === messageId);
            if (index !== -1) {
                messages.splice(index, 1);
                currentMessages[conversationId] = messages;
                this.messagesSubject.next(currentMessages);
                break;
            }
        }
    }

    private handleMessageEdited(message: Message): void {
        // Update edited message
        const currentMessages = this.messagesSubject.value;
        const conversationMessages = currentMessages[message.conversationId] || [];
        const index = conversationMessages.findIndex(m => m.id === message.id);
        if (index !== -1) {
            conversationMessages[index] = message;
            currentMessages[message.conversationId] = conversationMessages;
            this.messagesSubject.next(currentMessages);
        }
    }

    private updateUnreadCount(conversations: Conversation[]): void {
        const totalUnread = conversations.reduce((sum, conv) => sum + conv.unreadCount, 0);
        this.unreadCountSubject.next(totalUnread);
    }
}