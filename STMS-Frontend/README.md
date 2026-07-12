# STMSFrontend

# Part 1 : Install Angular 
## Installation Process
This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.2.13.

## Install Node Modules

```bash
npm install
```

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```


# Part 2 : Create Core Models

```ts
// src/app/core/models/user.model.ts
export interface User {
  id: string;
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  role: 'Admin' | 'ProjectManager' | 'TeamMember';
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

// src/app/core/models/auth.model.ts
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  role: string;
  expiresAt: Date;
}

export interface RefreshTokenRequest {
  token: string;
  refreshToken: string;
}

// src/app/core/models/project.model.ts
import { User } from "./user.model";

export interface Project {
    id: string;
    name: string;
    description: string;
    startDate: Date;
    endDate: Date | null;
    isActive: boolean;
    taskCount: number;
    completedTasks: number;
    createdByUser: User;
    createdAt: Date;
    updatedAt: Date;
}

export interface CreateProjectRequest {
    name: string;
    description: string;
    startDate: Date;
    endDate: Date | null;
}

export interface UpdateProjectRequest {
    name: string;
    description: string;
    startDate: Date;
    endDate: Date | null;
    isActive: boolean;
}

export interface ProjectFilter {
    searchTerm?: string;
    isActive?: boolean;
    startDateFrom?: Date;
    startDateTo?: Date;
    createdByUserId?: string;
    pageNumber: number;
    pageSize: number;
    sortBy: string;
    sortDescending: boolean;
}

export interface ProjectTaskSummary {
    projectId: string;
    projectName: string;
    totalTasks: number;
    completedTasks: number;
    inProgressTasks: number;
    toDoTasks: number;
    cancelledTasks: number;
    completionPercentage: number;
}

// src/app/core/models/task.model.ts
import { User } from "./user.model";

export interface Task {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: Date;
  estimatedHours: number;
  actualHours: number;
  projectId: string;
  projectName: string;
  assignedToUserId: string | null;
  assignedToUser: User | null;
  createdByUser: User;
  createdAt: Date;
  updatedAt: Date;
  isOverdue: boolean;
  daysUntilDue: number;
}

export enum TaskStatus {
  ToDo = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3
}

export enum TaskPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  priority: TaskPriority;
  dueDate: Date;
  estimatedHours: number;
  projectId: string;
  assignedToUserId: string | null;
}

export interface UpdateTaskRequest {
  title: string;
  description: string;
  priority: TaskPriority;
  dueDate: Date;
  estimatedHours: number;
  actualHours: number;
  assignedToUserId: string | null;
}

export interface UpdateTaskStatusRequest {
  status: TaskStatus;
  comment?: string;
}

export interface TaskFilter {
  searchTerm?: string;
  projectId?: string;
  assignedToUserId?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  dueDateFrom?: Date;
  dueDateTo?: Date;
  isOverdue?: boolean;
  showOnlyAssignedToMe?: boolean;
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  sortDescending: boolean;
}


// src/app/core/models/dashboard.model.ts
export interface DashboardSummary {
  projectStatistics: ProjectStatistics;
  taskStatistics: TaskStatistics;
  userStatistics: UserStatistics;
  recentActivity: ActivitySummary;
  performanceMetrics: PerformanceMetrics;
  generatedAt: Date;
}

export interface ProjectStatistics {
  totalProjects: number;
  activeProjects: number;
  inactiveProjects: number;
  projectsCompleted: number;
  projectsInProgress: number;
  projectCompletionRate: number;
  projectProgress: ProjectProgress[];
  projectsByMonth: Record<string, number>;
}

export interface ProjectProgress {
  projectId: string;
  projectName: string;
  progress: number;
  totalTasks: number;
  completedTasks: number;
  endDate: Date | null;
  isOverdue: boolean;
}

export interface TaskStatistics {
  totalTasks: number;
  completedTasks: number;
  inProgressTasks: number;
  toDoTasks: number;
  cancelledTasks: number;
  overdueTasks: number;
  tasksDueThisWeek: number;
  tasksDueNextWeek: number;
  completionRate: number;
  averageCompletionTime: number;
  tasksByStatus: Record<string, number>;
  tasksByPriority: Record<string, number>;
  tasksByAssignee: Record<string, number>;
  taskTrends: TaskTrend[];
}

export interface TaskTrend {
  period: Date;
  created: number;
  completed: number;
  inProgress: number;
}

export interface UserStatistics {
  totalUsers: number;
  activeUsers: number;
  inactiveUsers: number;
  usersByRole: Record<string, number>;
  usersByActivity: Record<string, number>;
  topPerformers: UserPerformance[];
}

export interface UserPerformance {
  userId: string;
  userName: string;
  fullName: string;
  assignedTasks: number;
  completedTasks: number;
  overdueTasks: number;
  completionRate: number;
  averageCompletionTime: number;
  productivityScore: number;
}

export interface ActivitySummary {
  recentActivities: RecentActivity[];
  activityTypes: Record<string, number>;
  activityTimeline: Record<string, number>;
}

export interface RecentActivity {
  timestamp: Date;
  user: string;
  action: string;
  entity: string;
  entityName: string;
  details?: string;
}

export interface PerformanceMetrics {
  overallProductivity: number;
  projectSuccessRate: number;
  taskEfficiency: number;
  onTimeDeliveryRate: number;
  resourceUtilization: number;
  metricsByProject: Record<string, number>;
  metricsTrend: { key: string; value: number }[];
}

// src/app/core/models/ai.model.ts
export interface TaskImprovementRequest {
  originalTitle: string;
  originalDescription: string;
  additionalContext?: string;
  options: ImprovementOptions;
}

export interface ImprovementOptions {
  correctGrammar: boolean;
  improveClarity: boolean;
  makeProfessional: boolean;
  expandDescription: boolean;
  makeActionable: boolean;
  maxLength: number;
  tone: string;
  language?: string;
}

export interface TaskImprovementResponse {
  improvedTitle: string;
  improvedDescription: string;
  summary: string;
  keyPoints: string[];
  suggestedActions: string[];
  metadata: ImprovementMetadata;
}

export interface ImprovementMetadata {
  model: string;
  originalLength: number;
  improvedLength: number;
  processingTimeSeconds: number;
  tokensUsed: number;
  processedAt: Date;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string[] | null;
  statusCode: number;
}

export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```
# Part 3 : Core Services

```ts
// src/app/core/services/api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiResponse } from '../models/ai.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly API_URL = 'https://localhost:5001/api';

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string, params?: any): Observable<T> {
    const httpParams = this.buildParams(params);
    return this.http.get<ApiResponse<T>>(`${this.API_URL}/${endpoint}`, { params: httpParams })
      .pipe(
        map(response => response.data),
        catchError(this.handleError)
      );
  }

  post<T>(endpoint: string, data?: any): Observable<T> {
    return this.http.post<ApiResponse<T>>(`${this.API_URL}/${endpoint}`, data)
      .pipe(
        map(response => response.data),
        catchError(this.handleError)
      );
  }

  put<T>(endpoint: string, data: any): Observable<T> {
    return this.http.put<ApiResponse<T>>(`${this.API_URL}/${endpoint}`, data)
      .pipe(
        map(response => response.data),
        catchError(this.handleError)
      );
  }

  patch<T>(endpoint: string, data: any): Observable<T> {
    return this.http.patch<ApiResponse<T>>(`${this.API_URL}/${endpoint}`, data)
      .pipe(
        map(response => response.data),
        catchError(this.handleError)
      );
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<ApiResponse<T>>(`${this.API_URL}/${endpoint}`)
      .pipe(
        map(response => response.data),
        catchError(this.handleError)
      );
  }

  private buildParams(params: any): HttpParams {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key] !== null && params[key] !== undefined) {
          httpParams = httpParams.append(key, params[key].toString());
        }
      });
    }
    return httpParams;
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An error occurred';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else {
      // Server-side error
      errorMessage = error.error?.message || error.message || `Error ${error.status}`;
    }
    return throwError(() => new Error(errorMessage));
  }
}


// src/app/core/services/auth.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { ApiService } from './api.service';
import { User } from '../models/user.model';
import { LoginRequest, RegisterRequest, AuthResponse, RefreshTokenRequest } from '../models/auth.model'

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'user_data';

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private apiService: ApiService,
    private router: Router
  ) {
    this.loadStoredUser();
  }

  login(loginRequest: LoginRequest): Observable<AuthResponse> {
    return this.apiService.post<AuthResponse>('auth/login', loginRequest)
      .pipe(
        tap(response => {
          this.setSession(response);
        })
      );
  }

  register(registerRequest: RegisterRequest): Observable<AuthResponse> {
    return this.apiService.post<AuthResponse>('auth/register', registerRequest)
      .pipe(
        tap(response => {
          this.setSession(response);
        })
      );
  }

  refreshToken(): Observable<AuthResponse> {
    const token = this.getToken();
    const refreshToken = this.getRefreshToken();
    
    if (!token || !refreshToken) {
      throw new Error('No tokens available');
    }

    const request: RefreshTokenRequest = { token, refreshToken };
    return this.apiService.post<AuthResponse>('auth/refresh', request)
      .pipe(
        tap(response => {
          this.setSession(response);
        })
      );
  }

  logout(): Observable<void> {
    const refreshToken = this.getRefreshToken();
    if (refreshToken) {
      this.apiService.post<void>('auth/logout', refreshToken).subscribe({
        error: () => this.clearSession()
      });
    }
    this.clearSession();
    this.router.navigate(['/login']);
    return new Observable<void>(observer => {
      observer.next();
      observer.complete();
    });
  }

  changePassword(currentPassword: string, newPassword: string): Observable<void> {
    return this.apiService.post<void>('auth/change-password', {
      currentPassword,
      newPassword,
      confirmNewPassword: newPassword
    });
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getUserRole(): string | null {
    return this.currentUserSubject.value?.role || null;
  }

  hasRole(role: string): boolean {
    return this.currentUserSubject.value?.role === role;
  }

  isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  isProjectManager(): boolean {
    return this.hasRole('ProjectManager') || this.isAdmin();
  }

  private setSession(authResponse: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, authResponse.token);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, authResponse.refreshToken);
    
    const user: User = {
      id: '', // This should be provided by the backend
      email: authResponse.email,
      username: authResponse.username,
      firstName: authResponse.firstName,
      lastName: authResponse.lastName,
      role: authResponse.role as any,
      isActive: true,
      createdAt: new Date(),
      updatedAt: new Date()
    };
    
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUserSubject.next(user);
    this.isAuthenticatedSubject.next(true);
  }

  private clearSession(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  private loadStoredUser(): void {
    const userData = localStorage.getItem(this.USER_KEY);
    const token = this.getToken();
    
    if (userData && token) {
      try {
        const user = JSON.parse(userData);
        this.currentUserSubject.next(user);
        this.isAuthenticatedSubject.next(true);
      } catch (e) {
        this.clearSession();
      }
    } else {
      this.isAuthenticatedSubject.next(false);
    }
  }
}


// src/app/core/services/notification.service.ts
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface Notification {
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
  duration?: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notificationSubject = new Subject<Notification>();
  public notifications$ = this.notificationSubject.asObservable();

  success(message: string, duration: number = 3000): void {
    this.notificationSubject.next({ type: 'success', message, duration });
  }

  error(message: string, duration: number = 5000): void {
    this.notificationSubject.next({ type: 'error', message, duration });
  }

  warning(message: string, duration: number = 4000): void {
    this.notificationSubject.next({ type: 'warning', message, duration });
  }

  info(message: string, duration: number = 3000): void {
    this.notificationSubject.next({ type: 'info', message, duration });
  }
}
```

# Part 4 : HTTP Interceptor
```ts
// src/app/core/interceptors/auth.interceptor.ts
import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, take, switchMap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject = new BehaviorSubject<string | null>(null);

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Skip adding token for auth endpoints
    if (request.url.includes('/auth/')) {
      return next.handle(request);
    }

    const token = this.authService.getToken();
    let req = request;

    if (token) {
      req = this.addToken(request, token);
    }

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !request.url.includes('/auth/refresh')) {
          return this.handle401Error(req, next);
        }
        return throwError(() => error);
      })
    );
  }

  private addToken(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response) => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(response.token);
          return next.handle(this.addToken(request, response.token));
        }),
        catchError((error) => {
          this.isRefreshing = false;
          this.authService.logout();
          return throwError(() => error);
        })
      );
    } else {
      return this.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(token => {
          return next.handle(this.addToken(request, token!));
        })
      );
    }
  }
}

// src/app/core/interceptors/error.interceptor.ts
import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private notificationService: NotificationService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unexpected error occurred';

        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = error.error.message;
        } else {
          // Server-side error
          errorMessage = error.error?.message || error.message;
          
          if (error.status === 0) {
            errorMessage = 'Unable to connect to the server';
          } else if (error.status === 404) {
            errorMessage = 'Resource not found';
          } else if (error.status === 500) {
            errorMessage = 'Internal server error';
          }
        }

        this.notificationService.error(errorMessage);
        return throwError(() => new Error(errorMessage));
      })
    );
  }
}
```