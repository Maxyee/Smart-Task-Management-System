import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../../core/services/task.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { AiService } from '../../../core/services/ai.service';
import { Task, TaskStatus, TaskPriority } from '../../../core/models/task.model';
import { Subject, takeUntil } from 'rxjs';

@Component({
    selector: 'app-task-detail',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './task-detail.component.html',
    styleUrls: ['./task-detail.component.css']
})
export class TaskDetailComponent implements OnInit, OnDestroy {
    task: Task | null = null;
    loading = true;
    isAdminOrManager = false;
    canEdit = false;

    // AI Improvement
    showAIImprovement = false;
    isImproving = false;
    improvementResult: any = null;
    aiOptions = {
        correctGrammar: true,
        improveClarity: true,
        makeProfessional: true,
        expandDescription: true,
        makeActionable: true,
        tone: 'Professional'
    };

    // Status update
    newStatus: TaskStatus | null = null;
    statusLabels = ['To Do', 'In Progress', 'Completed', 'Cancelled'];
    statusColors = ['gray', 'yellow', 'green', 'red'];
    priorityLabels = ['Low', 'Medium', 'High', 'Critical'];
    priorityColors = ['blue', 'green', 'orange', 'red'];

    private destroy$ = new Subject<void>();

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private taskService: TaskService,
        private authService: AuthService,
        private notificationService: NotificationService,
        private aiService: AiService
    ) { }

    ngOnInit(): void {
        const user = this.authService.getCurrentUser();
        this.isAdminOrManager = this.authService.isAdmin() || this.authService.isProjectManager();
        this.loadTask();
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    loadTask(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (!id) {
            this.router.navigate(['/tasks']);
            return;
        }

        this.loading = true;
        this.taskService.getTaskById(id)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (task) => {
                    this.task = task;
                    this.canEdit = this.isAdminOrManager ||
                        task.assignedToUserId === this.authService.getCurrentUser()?.id ||
                        task.createdByUser?.id === this.authService.getCurrentUser()?.id;
                    this.loading = false;
                },
                error: () => {
                    this.notificationService.error('Failed to load task');
                    this.loading = false;
                    this.router.navigate(['/tasks']);
                }
            });
    }

    updateStatus(status: TaskStatus): void {
        if (!this.task) return;

        this.taskService.updateTaskStatus(this.task.id, { status })
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: () => {
                    this.notificationService.success('Task status updated');
                    this.loadTask();
                },
                error: () => {
                    this.notificationService.error('Failed to update task status');
                }
            });
    }

    deleteTask(): void {
        if (!this.task) return;
        if (!confirm(`Are you sure you want to delete task "${this.task.title}"?`)) return;

        this.taskService.deleteTask(this.task.id)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: () => {
                    this.notificationService.success('Task deleted successfully');
                    this.router.navigate(['/tasks']);
                },
                error: () => {
                    this.notificationService.error('Failed to delete task');
                }
            });
    }

    improveWithAI(): void {
        if (!this.task) return;

        this.isImproving = true;
        this.aiService.improveTaskDescription({
            originalTitle: this.task.title,
            originalDescription: this.task.description,
            options: this.aiOptions
        }).pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (result) => {
                    this.improvementResult = result;
                    this.showAIImprovement = true;
                    this.isImproving = false;
                    this.notificationService.success('AI improvement generated');
                },
                error: () => {
                    this.isImproving = false;
                    this.notificationService.error('Failed to improve task description');
                }
            });
    }

    applyAIImprovement(): void {
        if (!this.task || !this.improvementResult) return;

        this.taskService.updateTask(this.task.id, {
            title: this.improvementResult.improvedTitle,
            description: this.improvementResult.improvedDescription,
            priority: this.task.priority,
            dueDate: this.task.dueDate,
            estimatedHours: this.task.estimatedHours,
            actualHours: this.task.actualHours,
            assignedToUserId: this.task.assignedToUserId
        }).pipe(takeUntil(this.destroy$))
            .subscribe({
                next: () => {
                    this.notificationService.success('Task updated with AI improvements');
                    this.showAIImprovement = false;
                    this.improvementResult = null;
                    this.loadTask();
                },
                error: () => {
                    this.notificationService.error('Failed to apply AI improvements');
                }
            });
    }

    dismissAIImprovement(): void {
        this.showAIImprovement = false;
        this.improvementResult = null;
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
}