import { User } from "./user.model";

export interface Task {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: Date;
  estimatedHours: number;
  actualHours: number;
  projectId: string;
  projectName: string;
  assignedToUserId: string | null;
  assignedToUser: User | null;
  createdByUser: User;
  createdAt: Date;
  updatedAt: Date;
  isOverdue: boolean;
  daysUntilDue: number;
}

export enum TaskStatus {
  ToDo = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3
}

export enum TaskPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  priority: TaskPriority;
  dueDate: Date;
  estimatedHours: number;
  projectId: string;
  assignedToUserId: string | null;
}

export interface UpdateTaskRequest {
  title: string;
  description: string;
  priority: TaskPriority;
  dueDate: Date;
  estimatedHours: number;
  actualHours: number;
  assignedToUserId: string | null;
}

export interface UpdateTaskStatusRequest {
  status: TaskStatus;
  comment?: string;
}

export interface TaskFilter {
  searchTerm?: string;
  projectId?: string;
  assignedToUserId?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  dueDateFrom?: Date;
  dueDateTo?: Date;
  isOverdue?: boolean;
  showOnlyAssignedToMe?: boolean;
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  sortDescending: boolean;
}

export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
