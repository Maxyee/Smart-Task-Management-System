import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { TaskService } from '../../../core/services/task.service';
import { ProjectService } from '../../../core/services/project.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { UserService } from '../../../core/services/user.service';
import { TaskPriority, TaskStatus } from '../../../core/models/task.model';
import { Project } from '../../../core/models/project.model';
import { User } from '../../../core/models/user.model';
import { finalize, takeUntil, debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { Observable, Subject, combineLatest, of } from 'rxjs';



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
    filteredUsers: User[] = [];
    projectId: string | null = null;

    // Search states
    userSearchTerm = '';
    isSearchingUsers = false;
    showUserDropdown = false;
    private blurTimeout: any = null;

    // Enums for template
    taskPriorities = Object.values(TaskPriority).filter(v => typeof v === 'number') as number[];
    priorityLabels = ['Low', 'Medium', 'High', 'Critical'];
    statusLabels = ['To Do', 'In Progress', 'Completed', 'Cancelled'];

    private destroy$ = new Subject<void>();
    private searchSubject = new Subject<string>();

    constructor(
        private fb: FormBuilder,
        private taskService: TaskService,
        private projectService: ProjectService,
        private userService: UserService,
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

        // Setup user search with debounce
        this.searchSubject.pipe(
            debounceTime(300),
            distinctUntilChanged(),
            switchMap(searchTerm => {
                this.isSearchingUsers = true;
                this.showUserDropdown = true;
                if (searchTerm && searchTerm.length >= 2) {
                    return this.userService.searchUsers(searchTerm);
                } else {
                    return of([]);
                }
            }),
            takeUntil(this.destroy$)
        ).subscribe({
            next: (users) => {
                this.filteredUsers = users;
                this.isSearchingUsers = false;
                this.showUserDropdown = users.length > 0 && this.userSearchTerm.length >= 2;
            },
            error: () => {
                this.isSearchingUsers = false;
                this.showUserDropdown = false;
            }
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

        // Load data
        combineLatest([
            this.loadProjects(),
            this.loadUsers()
        ]).pipe(takeUntil(this.destroy$)).subscribe();

        if (this.isEditMode) {
            this.loadTask();
        }

        // Watch for project changes to auto-assign if needed
        this.taskForm.get('projectId')?.valueChanges
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.updateAvailableUsers();
            });
    }

    ngOnDestroy(): void {
        // Clean up timeout on destroy
        if (this.blurTimeout) {
            clearTimeout(this.blurTimeout);
            this.blurTimeout = null;
        }
        this.destroy$.next();
        this.destroy$.complete();
    }

    loadProjects(): Observable<void> {
        return new Observable(observer => {
            this.projectService.getProjects({
                pageNumber: 1,
                pageSize: 100,
                sortBy: 'name',
                sortDescending: false
            })
                .pipe(takeUntil(this.destroy$))
                .subscribe({
                    next: (response) => {
                        this.projects = response.items;
                        observer.next();
                        observer.complete();
                    },
                    error: () => {
                        this.notificationService.error('Failed to load projects');
                        observer.error('Failed to load projects');
                    }
                });
        });
    }

    loadUsers(): Observable<void> {
        return new Observable(observer => {
            this.userService.getUsersForAssignment()
                .pipe(takeUntil(this.destroy$))
                .subscribe({
                    next: (users) => {
                        this.users = users;
                        this.filteredUsers = users;
                        observer.next();
                        observer.complete();
                    },
                    error: () => {
                        // Fallback to active users if endpoint not available
                        this.userService.getActiveUsers()
                            .pipe(takeUntil(this.destroy$))
                            .subscribe({
                                next: (users) => {
                                    this.users = users;
                                    this.filteredUsers = users;
                                    observer.next();
                                    observer.complete();
                                },
                                error: () => {
                                    // Fallback to current user only if all else fails
                                    const currentUser = this.authService.getCurrentUser();
                                    if (currentUser) {
                                        this.users = [currentUser];
                                        this.filteredUsers = [currentUser];
                                    }
                                    observer.next();
                                    observer.complete();
                                }
                            });
                    }
                });
        });
    }

    updateAvailableUsers(): void {
        // Filter users based on project if needed
        // For now, show all active users
        this.filteredUsers = this.users;
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

                    // Set the selected user in the dropdown
                    if (task.assignedToUserId) {
                        const assignedUser = this.users.find(u => u.id === task.assignedToUserId);
                        if (assignedUser) {
                            this.userSearchTerm = `${assignedUser.firstName} ${assignedUser.lastName} (${assignedUser.username})`;
                        }
                    }
                },
                error: () => {
                    this.notificationService.error('Failed to load task');
                    this.router.navigate(['/tasks']);
                }
            });
    }

    /**
     * Handle user search input
     */
    onUserSearch(event: Event): void {
        const value = (event.target as HTMLInputElement).value;
        this.userSearchTerm = value;

        // Clear any pending blur timeout
        if (this.blurTimeout) {
            clearTimeout(this.blurTimeout);
            this.blurTimeout = null;
        }

        this.searchSubject.next(value);
    }

    /**
     * Handle focus on user search input
     */
    onUserFocus(): void {
        // Clear any pending blur timeout
        if (this.blurTimeout) {
            clearTimeout(this.blurTimeout);
            this.blurTimeout = null;
        }

        // Show dropdown if we have search results
        if (this.userSearchTerm.length >= 2 && this.filteredUsers.length > 0) {
            this.showUserDropdown = true;
        }
    }

    /**
     * Handle blur on user search input
     */
    onUserBlur(): void {
        // Use setTimeout to allow click events on dropdown items to fire first
        this.blurTimeout = setTimeout(() => {
            this.showUserDropdown = false;
            this.blurTimeout = null;
        }, 200);
    }

    /**
     * Select a user from dropdown
     */
    selectUser(user: User): void {
        this.taskForm.patchValue({ assignedToUserId: user.id });
        this.userSearchTerm = `${user.firstName} ${user.lastName} (${user.username})`;
        this.showUserDropdown = false;

        // Clear any pending blur timeout
        if (this.blurTimeout) {
            clearTimeout(this.blurTimeout);
            this.blurTimeout = null;
        }
    }

    /**
     * Clear user selection
     */
    clearUserSelection(): void {
        this.taskForm.patchValue({ assignedToUserId: null });
        this.userSearchTerm = '';
        this.showUserDropdown = false;

        // Clear any pending blur timeout
        if (this.blurTimeout) {
            clearTimeout(this.blurTimeout);
            this.blurTimeout = null;
        }
    }

    onSubmit(): void {
        if (this.taskForm.invalid) {
            this.markFormGroupTouched(this.taskForm);
            this.notificationService.warning('Please fill in all required fields');
            return;
        }

        this.isLoading = true;
        const formData = this.taskForm.value;
        formData.dueDate = new Date(formData.dueDate);

        // Ensure assignedToUserId is null if empty
        if (!formData.assignedToUserId) {
            formData.assignedToUserId = null;
        }

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

    getUserDisplayName(user: User): string {
        return `${user.firstName} ${user.lastName} (${user.username})`;
    }

    getUserInitials(user: User): string {
        return `${user.firstName[0]}${user.lastName[0]}`;
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