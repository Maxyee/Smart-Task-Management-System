import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { ProjectService } from '../../../core/services/project.service';
import { NotificationService } from '../../../core/services/notification.service';
import { finalize, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-project-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './project-form.component.html',
  styleUrls: ['./project-form.component.css']
})
export class ProjectFormComponent implements OnInit {
  projectForm: FormGroup;
  isLoading = false;
  isEditMode = false;
  projectId: string | null = null;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private router: Router,
    private route: ActivatedRoute,
    private notificationService: NotificationService
  ) {
    this.projectForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      startDate: ['', [Validators.required]],
      endDate: [null],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.projectId;

    if (this.isEditMode) {
      this.loadProject();
    }
  }

  loadProject(): void {
    this.isLoading = true;
    this.projectService.getProjectById(this.projectId!)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isLoading = false)
      )
      .subscribe({
        next: (project) => {
          this.projectForm.patchValue({
            name: project.name,
            description: project.description,
            startDate: this.formatDate(project.startDate),
            endDate: project.endDate ? this.formatDate(project.endDate) : null,
            isActive: project.isActive
          });
        },
        error: () => {
          this.notificationService.error('Failed to load project');
          this.router.navigate(['/projects']);
        }
      });
  }

  onSubmit(): void {
    if (this.projectForm.invalid) {
      this.markFormGroupTouched(this.projectForm);
      return;
    }

    this.isLoading = true;
    const formData = this.projectForm.value;

    // Convert date strings to Date objects
    formData.startDate = new Date(formData.startDate);
    formData.endDate = formData.endDate ? new Date(formData.endDate) : null;

    const request = this.isEditMode
      ? this.projectService.updateProject(this.projectId!, formData)
      : this.projectService.createProject(formData);

    request.pipe(
      takeUntil(this.destroy$),
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        const message = this.isEditMode
          ? 'Project updated successfully'
          : 'Project created successfully';
        this.notificationService.success(message);
        this.router.navigate(['/projects']);
      },
      error: (error) => {
        this.notificationService.error(error.message || 'Failed to save project');
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/projects']);
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

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}