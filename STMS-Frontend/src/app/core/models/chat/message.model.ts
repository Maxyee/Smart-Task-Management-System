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