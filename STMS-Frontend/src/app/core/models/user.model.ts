import { UserPerformance } from "./dashboard.model";

export interface User {
  id: string;
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  role: 'Admin' | 'ProjectManager' | 'TeamMember';
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface UserFilter {
  searchTerm?: string;
  role?: string;
  isActive?: boolean;
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  sortDescending: boolean;
}

export interface UserStatistics {
  totalUsers: number;
  activeUsers: number;
  inactiveUsers: number;
  usersByRole: Record<string, number>;
  usersByActivity: Record<string, number>;
  topPerformers: UserPerformance[];
}