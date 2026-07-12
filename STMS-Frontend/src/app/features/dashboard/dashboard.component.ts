import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DashboardService } from '../../core/services/dashboard.service';
import { TaskService } from '../../core/services/task.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { 
  DashboardSummary, 
  ProjectProgress, 
  TaskStatistics,
  RecentActivity,
  UserPerformance,
  PerformanceMetrics,
} from '../../core/models/dashboard.model';
import{
  TaskStatus,
  TaskPriority
} from '../../core/models/task.model';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  // Dashboard Data
  dashboardSummary: DashboardSummary | null = null;
  loading = true;
  loadingTasks = false;
  
  // Filters
  daysToShow = 30;
  selectedProjectId: string | null = null;
  
  // Task Statistics
  taskStatistics: TaskStatistics | null = null;
  recentActivities: RecentActivity[] = [];
  topPerformers: UserPerformance[] = [];
  performanceMetrics: PerformanceMetrics | null = null;
  projectProgress: ProjectProgress[] = [];
  
  // UI State
  activeTab: 'overview' | 'tasks' | 'team' | 'metrics' = 'overview';
  isAdminOrManager = false;
  
  // Colors for charts
  statusColors = {
    [TaskStatus.ToDo]: '#9CA3AF',
    [TaskStatus.InProgress]: '#FBBF24',
    [TaskStatus.Completed]: '#34D399',
    [TaskStatus.Cancelled]: '#F87171'
  };
  
  priorityColors = {
    [TaskPriority.Low]: '#60A5FA',
    [TaskPriority.Medium]: '#34D399',
    [TaskPriority.High]: '#FB923C',
    [TaskPriority.Critical]: '#F87171'
  };

  private destroy$ = new Subject<void>();

  constructor(
    private dashboardService: DashboardService,
    private taskService: TaskService,
    private authService: AuthService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.isAdminOrManager = this.authService.isAdmin() || this.authService.isProjectManager();
    this.loadDashboard();
    this.loadTaskStatistics();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDashboard(): void {
    this.loading = true;
    const filter = {
      daysToShow: this.daysToShow,
      projectId: this.selectedProjectId || undefined
    };

    this.dashboardService.getDashboardSummary(filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (summary) => {
          this.dashboardSummary = summary;
          this.projectProgress = summary.projectStatistics.projectProgress;
          this.recentActivities = summary.recentActivity.recentActivities;
          this.topPerformers = summary.userStatistics.topPerformers;
          this.performanceMetrics = summary.performanceMetrics;
          this.loading = false;
        },
        error: () => {
          this.notificationService.error('Failed to load dashboard');
          this.loading = false;
        }
      });
  }

  loadTaskStatistics(): void {
    this.loadingTasks = true;
    this.taskService.getTaskStatistics(
      this.selectedProjectId || undefined,
      undefined
    ).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (stats) => {
          this.taskStatistics = stats;
          this.loadingTasks = false;
        },
        error: () => {
          this.loadingTasks = false;
        }
      });
  }

  refreshData(): void {
    this.loadDashboard();
    this.loadTaskStatistics();
  }

  changeTab(tab: 'overview' | 'tasks' | 'team' | 'metrics'): void {
    this.activeTab = tab;
  }

  getStatusLabel(status: number): string {
    const labels = ['To Do', 'In Progress', 'Completed', 'Cancelled'];
    return labels[status] || 'Unknown';
  }

  getPriorityLabel(priority: number): string {
    const labels = ['Low', 'Medium', 'High', 'Critical'];
    return labels[priority] || 'Unknown';
  }

  getStatusColor(status: number): string {
    return this.statusColors[status as TaskStatus] || '#9CA3AF';
  }

  getPriorityColor(priority: number): string {
    return this.priorityColors[priority as TaskPriority] || '#9CA3AF';
  }

  getProgressColor(percentage: number): string {
    if (percentage >= 80) return 'bg-green-500';
    if (percentage >= 50) return 'bg-yellow-500';
    if (percentage > 0) return 'bg-orange-500';
    return 'bg-gray-300';
  }

  getDaysUntilDue(dueDate: Date): number {
    const now = new Date();
    const due = new Date(dueDate);
    const diffTime = due.getTime() - now.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  getStatusBadgeClass(status: number): string {
    const classes = {
      [TaskStatus.ToDo]: 'bg-gray-100 text-gray-800',
      [TaskStatus.InProgress]: 'bg-yellow-100 text-yellow-800',
      [TaskStatus.Completed]: 'bg-green-100 text-green-800',
      [TaskStatus.Cancelled]: 'bg-red-100 text-red-800'
    };
    return classes[status as TaskStatus] || classes[TaskStatus.ToDo];
  }

  getPriorityBadgeClass(priority: number): string {
    const classes = {
      [TaskPriority.Low]: 'bg-blue-100 text-blue-800',
      [TaskPriority.Medium]: 'bg-green-100 text-green-800',
      [TaskPriority.High]: 'bg-orange-100 text-orange-800',
      [TaskPriority.Critical]: 'bg-red-100 text-red-800'
    };
    return classes[priority as TaskPriority] || classes[TaskPriority.Low];
  }

  // Helper for template iteration
  getKeys(obj: any): string[] {
    return Object.keys(obj);
  }

  getValues(obj: any): number[] {
    return Object.values(obj);
  }

  Math = Math;
  Object = Object;
}