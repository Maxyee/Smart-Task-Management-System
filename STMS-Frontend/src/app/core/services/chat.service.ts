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

@Injectable({
    providedIn: 'root'
})
export class ChatService implements OnDestroy {
    private hubConnection: signalR.HubConnection;

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
            .withUrl('https://localhost:5001/chathub', {
                accessTokenFactory: () => localStorage.getItem('access_token') || ''
            })
            .withAutomaticReconnect()
            .build();

        this.startConnection();
        this.registerHubEvents();
    }

    ngOnDestroy(): void {
        this.hubConnection.stop();
    }

    private startConnection(): void {
        this.hubConnection
            .start()
            .then(() => {
                console.log('SignalR connection established');
                this.loadConversations();
            })
            .catch(err => {
                console.error('SignalR connection failed:', err);
                // Retry after 5 seconds
                setTimeout(() => this.startConnection(), 5000);
            });
    }

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
    }

    // API Methods
    loadConversations(): void {
        this.apiService.get<Conversation[]>('chat/conversations')
            .subscribe({
                next: (conversations) => {
                    this.conversationsSubject.next(conversations);
                    this.updateUnreadCount(conversations);
                },
                error: (error) => console.error('Failed to load conversations:', error)
            });
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

    sendMessage(request: SendMessageRequest): void {
        this.hubConnection.invoke('SendMessage', request)
            .catch(err => console.error('Failed to send message:', err));
    }

    joinConversation(conversationId: string): void {
        this.hubConnection.invoke('JoinConversation', conversationId)
            .catch(err => console.error('Failed to join conversation:', err));
    }

    leaveConversation(conversationId: string): void {
        this.hubConnection.invoke('LeaveConversation', conversationId)
            .catch(err => console.error('Failed to leave conversation:', err));
    }

    markAsRead(conversationId: string, messageId: string): void {
        this.hubConnection.invoke('MarkAsRead', { conversationId, messageId })
            .catch(err => console.error('Failed to mark as read:', err));
    }

    sendTypingIndicator(conversationId: string, isTyping: boolean): void {
        this.hubConnection.invoke('TypingIndicator', { conversationId, isTyping })
            .catch(err => console.error('Failed to send typing indicator:', err));
    }

    deleteMessage(messageId: string): void {
        this.hubConnection.invoke('DeleteMessage', messageId)
            .catch(err => console.error('Failed to delete message:', err));
    }

    editMessage(messageId: string, newContent: string): void {
        this.hubConnection.invoke('EditMessage', messageId, newContent)
            .catch(err => console.error('Failed to edit message:', err));
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
        // This would need conversationId from the event
        // For now, we'll implement it differently
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