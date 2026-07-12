import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../../core/services/project.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Project, ProjectTaskSummary } from '../../../core/models/project.model';
import { Subject, takeUntil } from 'rxjs';

@Component({
    selector: 'app-project-detail',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './project-detail.component.html',
    styleUrls: ['./project-detail.component.css']
})
export class ProjectDetailComponent implements OnInit, OnDestroy {
    project: Project | null = null;
    summary: ProjectTaskSummary | null = null;
    loading = true;
    isAdminOrManager = false;
    private destroy$ = new Subject<void>();

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private projectService: ProjectService,
        private authService: AuthService,
        private notificationService: NotificationService
    ) { }

    ngOnInit(): void {
        this.isAdminOrManager = this.authService.isAdmin() || this.authService.isProjectManager();
        this.loadProject();
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    loadProject(): void {
        const id = this.route.snapshot.paramMap.get('id');
        if (!id) {
            this.router.navigate(['/projects']);
            return;
        }

        this.loading = true;
        this.projectService.getProjectById(id)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (project) => {
                    this.project = project;
                    this.loadSummary(id);
                },
                error: () => {
                    this.notificationService.error('Failed to load project');
                    this.loading = false;
                    this.router.navigate(['/projects']);
                }
            });
    }

    loadSummary(id: string): void {
        this.projectService.getProjectSummary(id)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (summary) => {
                    this.summary = summary;
                    this.loading = false;
                },
                error: () => {
                    this.loading = false;
                    // Summary is optional, don't show error
                }
            });
    }

    deleteProject(): void {
        if (!this.project) return;
        if (!confirm(`Are you sure you want to delete project "${this.project.name}"?`)) return;

        this.projectService.deleteProject(this.project.id)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: () => {
                    this.notificationService.success('Project deleted successfully');
                    this.router.navigate(['/projects']);
                },
                error: () => {
                    this.notificationService.error('Failed to delete project');
                }
            });
    }

    toggleStatus(): void {
        if (!this.project) return;
        this.projectService.toggleProjectStatus(this.project.id)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: () => {
                    this.notificationService.success('Project status updated');
                    this.loadProject();
                },
                error: () => {
                    this.notificationService.error('Failed to update project status');
                }
            });
    }

    getStatusBadgeClass(isActive: boolean): string {
        return isActive
            ? 'bg-green-100 text-green-800'
            : 'bg-gray-100 text-gray-800';
    }

    getStatusText(isActive: boolean): string {
        return isActive ? 'Active' : 'Inactive';
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
}