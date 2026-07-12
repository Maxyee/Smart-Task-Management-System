import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { TaskService } from '../../../core/services/task.service';
import { ProjectService } from '../../../core/services/project.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { TaskPriority, TaskStatus} from '../../../core/models/task.model';
import { Project } from '../../../core/models/project.model';
import { User } from '../../../core/models/user.model';
import { finalize, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
    selector: 'app-task-form',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule],
    templateUrl: './task-form.component.html',
    styleUrls: ['./task-form.component.css']
})
export class TaskFormComponent implements OnInit, OnDestroy {
    taskForm: FormGroup;
    isLoading = false;
    isEditMode = false;
    taskId: string | null = null;
    projects: Project[] = [];
    users: User[] = [];
    projectId: string | null = null;

    // Enums for template
    taskPriorities = Object.values(TaskPriority).filter(v => typeof v === 'number') as number[];
    priorityLabels = ['Low', 'Medium', 'High', 'Critical'];
    statusLabels = ['To Do', 'In Progress', 'Completed', 'Cancelled'];

    private destroy$ = new Subject<void>();

    constructor(
        private fb: FormBuilder,
        private taskService: TaskService,
        private projectService: ProjectService,
        private authService: AuthService,
        private router: Router,
        private route: ActivatedRoute,
        private notificationService: NotificationService
    ) {
        this.taskForm = this.fb.group({
            title: ['', [Validators.required, Validators.maxLength(200)]],
            description: ['', [Validators.maxLength(1000)]],
            priority: [TaskPriority.Medium, [Validators.required]],
            dueDate: ['', [Validators.required]],
            estimatedHours: [0, [Validators.min(0), Validators.max(999)]],
            projectId: ['', [Validators.required]],
            assignedToUserId: [null]
        });
    }

    ngOnInit(): void {
        // Get projectId from query params
        this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
            if (params['projectId']) {
                this.projectId = params['projectId'];
                this.taskForm.patchValue({ projectId: params['projectId'] });
            }
        });

        this.taskId = this.route.snapshot.paramMap.get('id');
        this.isEditMode = !!this.taskId;

        this.loadProjects();
        this.loadUsers();

        if (this.isEditMode) {
            this.loadTask();
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    loadProjects(): void {
        this.projectService.getProjects({ pageNumber: 1, pageSize: 100, sortBy: 'name', sortDescending: false })
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (response) => {
                    this.projects = response.items;
                },
                error: () => {
                    this.notificationService.error('Failed to load projects');
                }
            });
    }

    loadUsers(): void {
        // This would need a user service to get all users
        // For now, we'll use the current user
        const currentUser = this.authService.getCurrentUser();
        if (currentUser) {
            this.users = [currentUser];
        }
    }

    loadTask(): void {
        this.isLoading = true;
        this.taskService.getTaskById(this.taskId!)
            .pipe(
                takeUntil(this.destroy$),
                finalize(() => this.isLoading = false)
            )
            .subscribe({
                next: (task) => {
                    this.taskForm.patchValue({
                        title: task.title,
                        description: task.description,
                        priority: task.priority,
                        dueDate: this.formatDate(task.dueDate),
                        estimatedHours: task.estimatedHours,
                        projectId: task.projectId,
                        assignedToUserId: task.assignedToUserId
                    });
                },
                error: () => {
                    this.notificationService.error('Failed to load task');
                    this.router.navigate(['/tasks']);
                }
            });
    }

    onSubmit(): void {
        if (this.taskForm.invalid) {
            this.markFormGroupTouched(this.taskForm);
            return;
        }

        this.isLoading = true;
        const formData = this.taskForm.value;
        formData.dueDate = new Date(formData.dueDate);

        const request = this.isEditMode
            ? this.taskService.updateTask(this.taskId!, formData)
            : this.taskService.createTask(formData);

        request.pipe(
            takeUntil(this.destroy$),
            finalize(() => this.isLoading = false)
        ).subscribe({
            next: () => {
                const message = this.isEditMode
                    ? 'Task updated successfully'
                    : 'Task created successfully';
                this.notificationService.success(message);
                this.router.navigate(['/tasks']);
            },
            error: (error) => {
                this.notificationService.error(error.message || 'Failed to save task');
            }
        });
    }

    onCancel(): void {
        this.router.navigate(['/tasks']);
    }

    private markFormGroupTouched(formGroup: FormGroup): void {
        Object.values(formGroup.controls).forEach(control => {
            control.markAsTouched();
            if (control instanceof FormGroup) {
                this.markFormGroupTouched(control);
            }
        });
    }

    private formatDate(date: Date | string): string {
        if (!date) return '';
        const d = new Date(date);
        return d.toISOString().split('T')[0];
    }
}