import { ConversationType } from "../../enums/conversationType.enum";

export interface CreateConversationRequest {
  type: ConversationType;
  name?: string;
  participantIds: string[];
}