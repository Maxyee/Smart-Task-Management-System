import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../../../core/services/project.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Project, ProjectFilter, PagedResponse } from '../../../core/models/project.model';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.css']
})
export class ProjectListComponent implements OnInit, OnDestroy {
  projects: Project[] = [];
  loading = false;
  totalItems = 0;
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;

  filter: ProjectFilter = {
    pageNumber: 1,
    pageSize: 10,
    sortBy: 'createdAt',
    sortDescending: true
  };

  searchTerm = '';
  showFilters = false;
  isAdminOrManager = false;
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  constructor(
    private projectService: ProjectService,
    private authService: AuthService,
    private notificationService: NotificationService
  ) {
    // Setup search with debounce
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(searchTerm => {
      this.filter.searchTerm = searchTerm || undefined;
      this.filter.pageNumber = 1;
      this.loadProjects();
    });
  }

  ngOnInit(): void {
    this.isAdminOrManager = this.authService.isAdmin() || this.authService.isProjectManager();
    this.loadProjects();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadProjects(): void {
    this.loading = true;
    this.projectService.getProjects(this.filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: PagedResponse<Project>) => {
          this.projects = response.items;
          this.totalItems = response.totalCount;
          this.currentPage = response.pageNumber;
          this.pageSize = response.pageSize;
          this.totalPages = response.totalPages;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.notificationService.error('Failed to load projects');
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
    this.loadProjects();
  }

  applyFilters(): void {
    this.filter.pageNumber = 1;
    this.loadProjects();
    this.showFilters = false;
  }

  resetFilters(): void {
    this.filter.isActive = undefined;
    this.filter.startDateFrom = undefined;
    this.filter.startDateTo = undefined;
    this.filter.sortBy = 'createdAt';
    this.filter.sortDescending = true;
    this.filter.pageNumber = 1;
    this.loadProjects();
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.filter.pageNumber = page;
    this.loadProjects();
  }

  changeSort(field: string): void {
    if (this.filter.sortBy === field) {
      this.filter.sortDescending = !this.filter.sortDescending;
    } else {
      this.filter.sortBy = field;
      this.filter.sortDescending = true;
    }
    this.loadProjects();
  }

  getSortIcon(field: string): string {
    if (this.filter.sortBy !== field) return '↕';
    return this.filter.sortDescending ? '↓' : '↑';
  }

  deleteProject(id: string, name: string): void {
    if (!confirm(`Are you sure you want to delete project "${name}"?`)) return;

    this.projectService.deleteProject(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notificationService.success(`Project "${name}" deleted successfully`);
          this.loadProjects();
        },
        error: () => {
          this.notificationService.error('Failed to delete project');
        }
      });
  }

  toggleProjectStatus(id: string): void {
    this.projectService.toggleProjectStatus(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notificationService.success('Project status updated');
          this.loadProjects();
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
}