import { User } from "./user.model";

export interface Project {
    id: string;
    name: string;
    description: string;
    startDate: Date;
    endDate: Date | null;
    isActive: boolean;
    taskCount: number;
    completedTasks: number;
    createdByUser: User;
    createdAt: Date;
    updatedAt: Date;
}

export interface CreateProjectRequest {
    name: string;
    description: string;
    startDate: Date;
    endDate: Date | null;
}

export interface UpdateProjectRequest {
    name: string;
    description: string;
    startDate: Date;
    endDate: Date | null;
    isActive: boolean;
}

export interface ProjectFilter {
    searchTerm?: string;
    isActive?: boolean;
    startDateFrom?: Date;
    startDateTo?: Date;
    createdByUserId?: string;
    pageNumber: number;
    pageSize: number;
    sortBy: string;
    sortDescending: boolean;
}

export interface ProjectTaskSummary {
    projectId: string;
    projectName: string;
    totalTasks: number;
    completedTasks: number;
    inProgressTasks: number;
    toDoTasks: number;
    cancelledTasks: number;
    completionPercentage: number;
}
