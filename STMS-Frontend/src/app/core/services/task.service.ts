// src/app/core/services/task.service.ts

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
    Task,
    CreateTaskRequest,
    UpdateTaskRequest,
    UpdateTaskStatusRequest,
    TaskFilter,
    PagedResponse,
    TaskStatistics
} from '../models/task.model';

@Injectable({
    providedIn: 'root'
})
export class TaskService {
    constructor(private apiService: ApiService) { }

    getTasks(filter: TaskFilter): Observable<PagedResponse<Task>> {
        return this.apiService.get<PagedResponse<Task>>('tasks', filter);
    }

    getTaskById(id: string): Observable<Task> {
        return this.apiService.get<Task>(`tasks/${id}`);
    }

    createTask(request: CreateTaskRequest): Observable<Task> {
        return this.apiService.post<Task>('tasks', request);
    }

    updateTask(id: string, request: UpdateTaskRequest): Observable<Task> {
        return this.apiService.put<Task>(`tasks/${id}`, request);
    }

    deleteTask(id: string): Observable<void> {
        return this.apiService.delete<void>(`tasks/${id}`);
    }

    updateTaskStatus(id: string, request: UpdateTaskStatusRequest): Observable<Task> {
        return this.apiService.patch<Task>(`tasks/${id}/status`, request);
    }

    assignTask(id: string, userId: string): Observable<Task> {
        return this.apiService.patch<Task>(`tasks/${id}/assign`, { userId });
    }

    getTaskStatistics(projectId?: string, userId?: string): Observable<TaskStatistics> {
        const params: any = {};
        if (projectId) params.projectId = projectId;
        if (userId) params.userId = userId;
        return this.apiService.get<TaskStatistics>('tasks/statistics', params);
    }

    getOverdueTasks(): Observable<Task[]> {
        return this.apiService.get<Task[]>('tasks/overdue');
    }

    getTasksDueSoon(daysThreshold: number = 7): Observable<Task[]> {
        return this.apiService.get<Task[]>('tasks/due-soon', { daysThreshold });
    }

    bulkUpdateStatus(taskIds: string[], newStatus: number): Observable<void> {
        return this.apiService.patch<void>('tasks/bulk-status', { taskIds, newStatus });
    }
}