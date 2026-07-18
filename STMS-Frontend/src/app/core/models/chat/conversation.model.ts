import { ConversationType } from "../../enums/conversationType.enum";
import { Message } from "./message.model";
import { Participant } from "./participant.model";


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