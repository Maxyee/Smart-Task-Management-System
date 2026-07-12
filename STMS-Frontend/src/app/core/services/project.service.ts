import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Project,
  CreateProjectRequest,
  UpdateProjectRequest,
  ProjectFilter,
  PagedResponse,
  ProjectTaskSummary
} from '../models/project.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  constructor(private apiService: ApiService) {}

  getProjects(filter: ProjectFilter): Observable<PagedResponse<Project>> {
    return this.apiService.get<PagedResponse<Project>>('projects', filter);
  }

  getProjectById(id: string): Observable<Project> {
    return this.apiService.get<Project>(`projects/${id}`);
  }

  createProject(request: CreateProjectRequest): Observable<Project> {
    return this.apiService.post<Project>('projects', request);
  }

  updateProject(id: string, request: UpdateProjectRequest): Observable<Project> {
    return this.apiService.put<Project>(`projects/${id}`, request);
  }

  deleteProject(id: string): Observable<void> {
    return this.apiService.delete<void>(`projects/${id}`);
  }

  toggleProjectStatus(id: string): Observable<void> {
    return this.apiService.patch<void>(`projects/${id}/toggle-status`, {});
  }

  getProjectSummary(id: string): Observable<ProjectTaskSummary> {
    return this.apiService.get<ProjectTaskSummary>(`projects/${id}/summary`);
  }

  getProjectsProgress(): Observable<Project[]> {
    return this.apiService.get<Project[]>('dashboard/projects/progress');
  }
}