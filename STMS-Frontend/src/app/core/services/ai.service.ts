import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { TaskImprovementRequest, TaskImprovementResponse } from '../models/ai.model';

@Injectable({
  providedIn: 'root'
})
export class AiService {
  constructor(private apiService: ApiService) {}

  improveTaskDescription(request: TaskImprovementRequest): Observable<TaskImprovementResponse> {
    return this.apiService.post<TaskImprovementResponse>('ai/improve-task', request);
  }

  generateSummary(description: string): Observable<string> {
    return this.apiService.post<string>('ai/summarize', { description });
  }

  suggestActions(title: string, description: string, status: string): Observable<string[]> {
    return this.apiService.post<string[]>('ai/suggest-actions', { title, description, status });
  }

  checkHealth(): Observable<boolean> {
    return this.apiService.get<boolean>('ai/health');
  }

  // New: Bulk improvement
  bulkImprove(taskIds: string[], options: any): Observable<any> {
    return this.apiService.post<any>('ai/bulk-improve', { taskIds, options });
  }

  // New: Preview improvement without saving
  previewImprovement(request: TaskImprovementRequest): Observable<TaskImprovementResponse> {
    return this.apiService.post<TaskImprovementResponse>('ai/preview', request);
  }
}