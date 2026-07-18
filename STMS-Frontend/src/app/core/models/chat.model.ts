export interface Conversation {
  id: string;
  name: string;
  type: ConversationType;
  avatarUrl?: string;
  lastMessageAt?: Date;
  lastMessage?: Message;
  unreadCount: number;
  participants: Participant[];
}

export enum ConversationType {
  Direct = 0,
  Group = 1
}

export interface Message {
  id: string;
  conversationId: string;
  senderId: string;
  senderName: string;
  senderAvatar: string;
  content: string;
  type: MessageType;
  sentAt: Date;
  readAt?: Date;
  isOwn: boolean;
  isRead: boolean;
  attachments?: MessageAttachment[];
}

export enum MessageType {
  Text = 0,
  Image = 1,
  File = 2,
  System = 3
}

export interface MessageAttachment {
  id: string;
  fileName: string;
  fileUrl: string;
  fileType: string;
  fileSize: number;
}

export interface Participant {
  userId: string;
  userName: string;
  fullName: string;
  avatarUrl: string;
  isAdmin: boolean;
  isOnline: boolean;
  lastSeenAt?: Date;
}

export interface CreateConversationRequest {
  type: ConversationType;
  name?: string;
  participantIds: string[];
}

export interface SendMessageRequest {
  conversationId: string;
  content: string;
  type: MessageType;
  replyToMessageId?: string;
}

export interface TypingIndicator {
  conversationId: string;
  isTyping: boolean;
}

export interface UserPresence {
  userId: string;
  isOnline: boolean;
  lastSeenAt?: Date;
}