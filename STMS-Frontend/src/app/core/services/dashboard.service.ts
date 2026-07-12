import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { DashboardSummary, ProjectProgress, UserPerformance, PerformanceMetrics, RecentActivity } from '../models/dashboard.model';

@Injectable({
    providedIn: 'root'
})
export class DashboardService {
    constructor(private apiService: ApiService) { }

    getDashboardSummary(filter: { daysToShow: number; projectId?: string }): Observable<DashboardSummary> {
        return this.apiService.get<DashboardSummary>('dashboard/summary', filter);
    }

    getProjectProgress(projectId: string): Observable<ProjectProgress> {
        return this.apiService.get<ProjectProgress>(`dashboard/projects/${projectId}/progress`);
    }

    getAllProjectsProgress(): Observable<ProjectProgress[]> {
        return this.apiService.get<ProjectProgress[]>('dashboard/projects/progress');
    }

    getUserPerformance(userId: string): Observable<UserPerformance> {
        return this.apiService.get<UserPerformance>(`dashboard/users/${userId}/performance`);
    }

    getTeamPerformance(): Observable<UserPerformance[]> {
        return this.apiService.get<UserPerformance[]>('dashboard/team/performance');
    }

    getPerformanceMetrics(filter: { daysToShow: number; projectId?: string }): Observable<PerformanceMetrics> {
        return this.apiService.get<PerformanceMetrics>('dashboard/metrics', filter);
    }

    getRecentActivities(count: number = 10, userId?: string): Observable<RecentActivity[]> {
        const params: any = { count };
        if (userId) params.userId = userId;
        return this.apiService.get<RecentActivity[]>('dashboard/activities', params);
    }
}