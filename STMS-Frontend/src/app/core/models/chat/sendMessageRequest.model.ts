import { MessageType } from "../../enums/messageType.enum";

export interface SendMessageRequest {
  conversationId: string;
  content: string;
  type: MessageType;
  replyToMessageId?: string;
}