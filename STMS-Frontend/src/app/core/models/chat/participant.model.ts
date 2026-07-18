export interface Participant {
  userId: string;
  userName: string;
  fullName: string;
  avatarUrl: string;
  isAdmin: boolean;
  isOnline: boolean;
  lastSeenAt?: Date;
}