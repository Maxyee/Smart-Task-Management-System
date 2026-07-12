import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Task, TaskFilter, PagedResponse, TaskStatus, TaskPriority } from '../../../core/models/task.model';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';

@Component({
    selector: 'app-task-list',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './task-list.component.html',
    styleUrls: ['./task-list.component.css']
})
export class TaskListComponent implements OnInit, OnDestroy {
    tasks: Task[] = [];
    loading = false;
    totalItems = 0;
    currentPage = 1;
    pageSize = 10;
    totalPages = 0;

    filter: TaskFilter = {
        pageNumber: 1,
        pageSize: 10,
        sortBy: 'dueDate',
        sortDescending: false
    };

    searchTerm = '';
    showFilters = false;
    isAdminOrManager = false;
    projectId: string | null = null;

    // Available filter options
    taskStatuses = Object.values(TaskStatus).filter(v => typeof v === 'number') as number[];
    taskPriorities = Object.values(TaskPriority).filter(v => typeof v === 'number') as number[];
    statusLabels = ['To Do', 'In Progress', 'Completed', 'Cancelled'];
    priorityLabels = ['Low', 'Medium', 'High', 'Critical'];

    // Bulk operations
    selectedTasks: Set<string> = new Set();
    bulkStatus: TaskStatus | null = null;
    showBulkActions = false;

    private destroy$ = new Subject<void>();
    private searchSubject = new Subject<string>();

    constructor(
        private taskService: TaskService,
        private authService: AuthService,
        private notificationService: NotificationService,
        private route: ActivatedRoute
    ) {
        this.searchSubject.pipe(
            debounceTime(300),
            distinctUntilChanged(),
            takeUntil(this.destroy$)
        ).subscribe(searchTerm => {
            this.filter.searchTerm = searchTerm || undefined;
            this.filter.pageNumber = 1;
            this.loadTasks();
        });
    }

    ngOnInit(): void {
        this.isAdminOrManager = this.authService.isAdmin() || this.authService.isProjectManager();

        // Check for projectId from query params
        this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
            if (params['projectId']) {
                this.projectId = params['projectId'];
                this.filter.projectId = params['projectId'];
                this.loadTasks();
            }
        });

        if (!this.projectId) {
            this.loadTasks();
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    loadTasks(): void {
        this.loading = true;
        this.taskService.getTasks(this.filter)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (response: PagedResponse<Task>) => {
                    this.tasks = response.items;
                    this.totalItems = response.totalCount;
                    this.currentPage = response.pageNumber;
                    this.pageSize = response.pageSize;
                    this.totalPages = response.totalPages;
                    this.loading = false;
                    this.selectedTasks.clear();
                },
                error: () => {
                    this.loading = false;
                    this.notificationService.error('Failed to load tasks');
                }
            });
    }

    onSearch(event: Event): void {
        const value = (event.target as HTMLInputElement).value;
        this.searchSubject.next(value);
    }

    clearSearch(): void {
        this.searchTerm = '';
        this.filter.searchTerm = undefined;
        this.filter.pageNumber = 1;
        this.loadTasks();
    }

    applyFilters(): void {
        this.filter.pageNumber = 1;
        this.loadTasks();
        this.showFilters = false;
    }

    resetFilters(): void {
        this.filter.status = undefined;
        this.filter.priority = undefined;
        this.filter.dueDateFrom = undefined;
        this.filter.dueDateTo = undefined;
        this.filter.isOverdue = undefined;
        this.filter.showOnlyAssignedToMe = undefined;
        this.filter.sortBy = 'dueDate';
        this.filter.sortDescending = false;
        this.filter.pageNumber = 1;
        this.loadTasks();
    }

    changePage(page: number): void {
        if (page < 1 || page > this.totalPages) return;
        this.filter.pageNumber = page;
        this.loadTasks();
    }

    changeSort(field: string): void {
        if (this.filter.sortBy === field) {
            this.filter.sortDescending = !this.filter.sortDescending;
        } else {
            this.filter.sortBy = field;
            this.filter.sortDescending = false;
        }
        this.loadTasks();
    }

    getSortIcon(field: string): string {
        if (this.filter.sortBy !== field) return '↕';
        return this.filter.sortDescending ? '↓' : '↑';
    }

    deleteTask(id: string, title: string): void {
        if (!confirm(`Are you sure you want to delete task "${title}"?`)) return;

        this.taskService.deleteTask(id)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: () => {
                    this.notificationService.success(`Task "${title}" deleted successfully`);
                    this.loadTasks();
                },
                error: () => {
                    this.notificationService.error('Failed to delete task');
                }
            });
    }

    updateTaskStatus(id: string, status: TaskStatus): void {
        this.taskService.updateTaskStatus(id, { status })
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: () => {
                    this.notificationService.success('Task status updated');
                    this.loadTasks();
                },
                error: () => {
                    this.notificationService.error('Failed to update task status');
                }
            });
    }

    toggleTaskSelection(id: string): void {
        if (this.selectedTasks.has(id)) {
            this.selectedTasks.delete(id);
        } else {
            this.selectedTasks.add(id);
        }
        this.showBulkActions = this.selectedTasks.size > 0;
    }

    selectAllTasks(): void {
        if (this.selectedTasks.size === this.tasks.length) {
            this.selectedTasks.clear();
        } else {
            this.tasks.forEach(task => this.selectedTasks.add(task.id));
        }
        this.showBulkActions = this.selectedTasks.size > 0;
    }

    applyBulkStatus(): void {
        if (!this.bulkStatus || this.selectedTasks.size === 0) return;

        if (!confirm(`Apply status to ${this.selectedTasks.size} tasks?`)) return;

        this.taskService.bulkUpdateStatus(Array.from(this.selectedTasks), this.bulkStatus)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: () => {
                    this.notificationService.success('Bulk status update completed');
                    this.selectedTasks.clear();
                    this.showBulkActions = false;
                    this.bulkStatus = null;
                    this.loadTasks();
                },
                error: () => {
                    this.notificationService.error('Failed to update tasks');
                }
            });
    }

    getStatusBadgeClass(status: TaskStatus): string {
        const classes = {
            [TaskStatus.ToDo]: 'bg-gray-100 text-gray-800',
            [TaskStatus.InProgress]: 'bg-yellow-100 text-yellow-800',
            [TaskStatus.Completed]: 'bg-green-100 text-green-800',
            [TaskStatus.Cancelled]: 'bg-red-100 text-red-800'
        };
        return classes[status] || classes[TaskStatus.ToDo];
    }

    getStatusText(status: TaskStatus): string {
        return this.statusLabels[status] || 'Unknown';
    }

    getPriorityBadgeClass(priority: TaskPriority): string {
        const classes = {
            [TaskPriority.Low]: 'bg-blue-100 text-blue-800',
            [TaskPriority.Medium]: 'bg-green-100 text-green-800',
            [TaskPriority.High]: 'bg-orange-100 text-orange-800',
            [TaskPriority.Critical]: 'bg-red-100 text-red-800'
        };
        return classes[priority] || classes[TaskPriority.Low];
    }

    getPriorityText(priority: TaskPriority): string {
        return this.priorityLabels[priority] || 'Unknown';
    }

    getDaysUntilDue(dueDate: Date): number {
        const now = new Date();
        const due = new Date(dueDate);
        const diffTime = due.getTime() - now.getTime();
        return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    }

    getDueDateClass(dueDate: Date): string {
        const days = this.getDaysUntilDue(dueDate);
        if (days < 0) return 'text-red-600 font-medium';
        if (days <= 3) return 'text-orange-600';
        return 'text-gray-500';
    }

    getPageNumbers(): number[] {
        const pages = [];
        const maxVisible = 5;
        let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
        let end = Math.min(this.totalPages, start + maxVisible - 1);

        if (end - start + 1 < maxVisible) {
            start = Math.max(1, end - maxVisible + 1);
        }

        for (let i = start; i <= end; i++) {
            pages.push(i);
        }
        return pages;
    }

    // Helper for template Math
    Math = Math;
}