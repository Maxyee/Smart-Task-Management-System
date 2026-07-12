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

# Part 5 : Route Guards
```ts
// src/app/core/guards/auth.guard.ts
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.authService.isAuthenticated$.pipe(
      take(1),
      map(isAuthenticated => {
        if (isAuthenticated) {
          return true;
        }
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
      })
    );
  }
}


// src/app/core/guards/role.guard.ts
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    const requiredRoles = route.data['roles'] as string[];
    
    return this.authService.currentUser$.pipe(
      take(1),
      map(user => {
        if (!user) {
          this.router.navigate(['/login']);
          return false;
        }

        if (requiredRoles && requiredRoles.length > 0) {
          const hasRole = requiredRoles.includes(user.role);
          if (!hasRole) {
            this.router.navigate(['/dashboard']);
            return false;
          }
        }

        return true;
      })
    );
  }
}
```

# Part 6 : Auth Module (Login/Register)
```ts
// src/app/features/auth/login/login.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  isLoading = false;
  submitted = false;
  returnUrl = '/dashboard';
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  ngOnInit(): void {
    // Redirect if already logged in
    this.authService.isAuthenticated$.subscribe(isAuth => {
      if (isAuth) {
        this.router.navigate(['/dashboard']);
      }
    });
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    const { email, password, rememberMe } = this.loginForm.value;

    this.authService.login({ email, password })
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: () => {
          this.notificationService.success('Login successful!');
          this.router.navigate([this.returnUrl]);
        },
        error: (error) => {
          this.notificationService.error(error.message || 'Login failed');
        }
      });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
}
```

```html
<!-- src/app/features/auth/login/login.component.html -->
<div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
        <div>
            <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
                Smart Task Management
            </h2>
            <p class="mt-2 text-center text-sm text-gray-600">
                Sign in to your account
            </p>
        </div>

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="mt-8 space-y-6">
            <div class="rounded-md shadow-sm -space-y-px">
                <div>
                    <label for="email" class="sr-only">Email address</label>
                    <input id="email" type="email" formControlName="email"
                        class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-t-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
                        placeholder="Email address"
                        [class.border-red-500]="submitted && loginForm.get('email')?.invalid" />
                    <div *ngIf="submitted && loginForm.get('email')?.invalid" class="text-red-500 text-xs mt-1">
                        <span *ngIf="loginForm.get('email')?.errors?.['required']">Email is required</span>
                        <span *ngIf="loginForm.get('email')?.errors?.['email']">Please enter a valid email</span>
                    </div>
                </div>

                <div class="relative">
                    <label for="password" class="sr-only">Password</label>
                    <input id="password" [type]="showPassword ? 'text' : 'password'" formControlName="password"
                        class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-b-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
                        placeholder="Password"
                        [class.border-red-500]="submitted && loginForm.get('password')?.invalid" />
                    <button type="button" (click)="togglePasswordVisibility()"
                        class="absolute inset-y-0 right-0 pr-3 flex items-center text-sm leading-5">
                        <span class="text-gray-500">{{ showPassword ? 'Hide' : 'Show' }}</span>
                    </button>
                    <div *ngIf="submitted && loginForm.get('password')?.invalid" class="text-red-500 text-xs mt-1">
                        <span *ngIf="loginForm.get('password')?.errors?.['required']">Password is required</span>
                        <span *ngIf="loginForm.get('password')?.errors?.['minlength']">Password must be at least 6
                            characters</span>
                    </div>
                </div>
            </div>

            <div class="flex items-center justify-between">
                <div class="flex items-center">
                    <input id="remember-me" type="checkbox" formControlName="rememberMe"
                        class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded" />
                    <label for="remember-me" class="ml-2 block text-sm text-gray-900">
                        Remember me
                    </label>
                </div>

                <div class="text-sm">
                    <a routerLink="/forgot-password" class="font-medium text-indigo-600 hover:text-indigo-500">
                        Forgot your password?
                    </a>
                </div>
            </div>

            <div>
                <button type="submit" [disabled]="isLoading"
                    class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50">
                    <span class="absolute left-0 inset-y-0 flex items-center pl-3">
                        <svg *ngIf="!isLoading" class="h-5 w-5 text-indigo-500 group-hover:text-indigo-400"
                            xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor"
                            aria-hidden="true">
                            <path fill-rule="evenodd"
                                d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z"
                                clip-rule="evenodd" />
                        </svg>
                        <svg *ngIf="isLoading" class="animate-spin h-5 w-5 text-indigo-500"
                            xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4">
                            </circle>
                            <path class="opacity-75" fill="currentColor"
                                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                            </path>
                        </svg>
                    </span>
                    {{ isLoading ? 'Signing in...' : 'Sign in' }}
                </button>
            </div>

            <div class="text-center">
                <p class="text-sm text-gray-600">
                    Don't have an account?
                    <a routerLink="/register" class="font-medium text-indigo-600 hover:text-indigo-500">
                        Register here
                    </a>
                </p>
            </div>
        </form>
    </div>
</div>
```
# Part 7 :  App Configuration

```ts
// src/app/app.config.ts
import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection  } from '@angular/core';
import { provideRouter, withComponentInputBinding, withRouterConfig } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideBrowserGlobalErrorListeners(),
    provideRouter(
      routes,
      withComponentInputBinding(),
      withRouterConfig({
        paramsInheritanceStrategy: 'always'
      })
    ),
     provideHttpClient(withInterceptorsFromDi())
  ]
};

```

# Part 8 : App Routing

```ts
// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'projects',
    loadComponent: () => import('./features/projects/project-list/project-list.component').then(m => m.ProjectListComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'projects/create',
    loadComponent: () => import('./features/projects/project-form/project-form.component').then(m => m.ProjectFormComponent),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'ProjectManager'] }
  },
  {
    path: 'projects/:id',
    loadComponent: () => import('./features/projects/project-detail/project-detail.component').then(m => m.ProjectDetailComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'projects/:id/edit',
    loadComponent: () => import('./features/projects/project-form/project-form.component').then(m => m.ProjectFormComponent),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin', 'ProjectManager'] }
  },
  {
    path: 'tasks',
    loadComponent: () => import('./features/tasks/task-list/task-list.component').then(m => m.TaskListComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'tasks/create',
    loadComponent: () => import('./features/tasks/task-form/task-form.component').then(m => m.TaskFormComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'tasks/:id',
    loadComponent: () => import('./features/tasks/task-detail/task-detail.component').then(m => m.TaskDetailComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'tasks/:id/edit',
    loadComponent: () => import('./features/tasks/task-form/task-form.component').then(m => m.TaskFormComponent),
    canActivate: [AuthGuard]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];

```
# Part 9 : Main App and Shared Component

```ts
// src/app/app.ts
import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './shared/components/header/header.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { NotificationComponent } from './shared/components/notification/notification.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    NotificationComponent
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('STMS-Frontend');
}


// src/app/shared/components/header/header.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  user: User | null = null;
  isMenuOpen = false;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.user = user;
    });
  }

  logout(): void {
    this.authService.logout();
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  getInitials(): string {
    if (!this.user) return '';
    return `${this.user.firstName[0]}${this.user.lastName[0]}`;
  }
}
```

# Part 10 : Register, Footer, Notification component
```ts
// src/app/features/auth/register/register.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { finalize } from 'rxjs/operators';


@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  isLoading = false;
  submitted = false;
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      acceptTerms: [false, [Validators.requiredTrue]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  ngOnInit(): void {
    // Redirect if already logged in
    this.authService.isAuthenticated$.subscribe(isAuth => {
      if (isAuth) {
        this.router.navigate(['/dashboard']);
      }
    });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;
    const { firstName, lastName, username, email, password, confirmPassword } = this.registerForm.value;

    this.authService.register({
      firstName,
      lastName,
      username,
      email,
      password,
      confirmPassword
    }).pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: () => {
          this.notificationService.success('Registration successful! Welcome to STMS.');
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.notificationService.error(error.message || 'Registration failed. Please try again.');
        }
      });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
}
```

```html
<div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
  <div class="max-w-md w-full space-y-8">
    <div>
      <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
        Create your account
      </h2>
      <p class="mt-2 text-center text-sm text-gray-600">
        Join Smart Task Management System
      </p>
    </div>

    <form [formGroup]="registerForm" (ngSubmit)="onSubmit()" class="mt-8 space-y-6">
      <div class="rounded-md shadow-sm space-y-4">
        <!-- First Name & Last Name -->
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label for="firstName" class="block text-sm font-medium text-gray-700">First Name</label>
            <input
              id="firstName"
              type="text"
              formControlName="firstName"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              [class.border-red-500]="submitted && registerForm.get('firstName')?.invalid"
            />
            <div *ngIf="submitted && registerForm.get('firstName')?.invalid" class="text-red-500 text-xs mt-1">
              <span *ngIf="registerForm.get('firstName')?.errors?.['required']">First name is required</span>
              <span *ngIf="registerForm.get('firstName')?.errors?.['minlength']">Minimum 2 characters</span>
            </div>
          </div>

          <div>
            <label for="lastName" class="block text-sm font-medium text-gray-700">Last Name</label>
            <input
              id="lastName"
              type="text"
              formControlName="lastName"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              [class.border-red-500]="submitted && registerForm.get('lastName')?.invalid"
            />
            <div *ngIf="submitted && registerForm.get('lastName')?.invalid" class="text-red-500 text-xs mt-1">
              <span *ngIf="registerForm.get('lastName')?.errors?.['required']">Last name is required</span>
              <span *ngIf="registerForm.get('lastName')?.errors?.['minlength']">Minimum 2 characters</span>
            </div>
          </div>
        </div>

        <!-- Username -->
        <div>
          <label for="username" class="block text-sm font-medium text-gray-700">Username</label>
          <input
            id="username"
            type="text"
            formControlName="username"
            class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
            [class.border-red-500]="submitted && registerForm.get('username')?.invalid"
          />
          <div *ngIf="submitted && registerForm.get('username')?.invalid" class="text-red-500 text-xs mt-1">
            <span *ngIf="registerForm.get('username')?.errors?.['required']">Username is required</span>
            <span *ngIf="registerForm.get('username')?.errors?.['minlength']">Minimum 3 characters</span>
          </div>
        </div>

        <!-- Email -->
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700">Email Address</label>
          <input
            id="email"
            type="email"
            formControlName="email"
            class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
            [class.border-red-500]="submitted && registerForm.get('email')?.invalid"
          />
          <div *ngIf="submitted && registerForm.get('email')?.invalid" class="text-red-500 text-xs mt-1">
            <span *ngIf="registerForm.get('email')?.errors?.['required']">Email is required</span>
            <span *ngIf="registerForm.get('email')?.errors?.['email']">Please enter a valid email</span>
          </div>
        </div>

        <!-- Password -->
        <div>
          <label for="password" class="block text-sm font-medium text-gray-700">Password</label>
          <div class="relative">
            <input
              id="password"
              [type]="showPassword ? 'text' : 'password'"
              formControlName="password"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              [class.border-red-500]="submitted && registerForm.get('password')?.invalid"
            />
            <button
              type="button"
              (click)="togglePasswordVisibility()"
              class="absolute inset-y-0 right-0 pr-3 flex items-center text-sm leading-5"
            >
              <span class="text-gray-500">{{ showPassword ? 'Hide' : 'Show' }}</span>
            </button>
          </div>
          <div *ngIf="submitted && registerForm.get('password')?.invalid" class="text-red-500 text-xs mt-1">
            <span *ngIf="registerForm.get('password')?.errors?.['required']">Password is required</span>
            <span *ngIf="registerForm.get('password')?.errors?.['minlength']">Minimum 6 characters</span>
          </div>
        </div>

        <!-- Confirm Password -->
        <div>
          <label for="confirmPassword" class="block text-sm font-medium text-gray-700">Confirm Password</label>
          <div class="relative">
            <input
              id="confirmPassword"
              [type]="showConfirmPassword ? 'text' : 'password'"
              formControlName="confirmPassword"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              [class.border-red-500]="submitted && registerForm.get('confirmPassword')?.invalid"
            />
            <button
              type="button"
              (click)="toggleConfirmPasswordVisibility()"
              class="absolute inset-y-0 right-0 pr-3 flex items-center text-sm leading-5"
            >
              <span class="text-gray-500">{{ showConfirmPassword ? 'Hide' : 'Show' }}</span>
            </button>
          </div>
          <div *ngIf="submitted && registerForm.get('confirmPassword')?.invalid" class="text-red-500 text-xs mt-1">
            <span *ngIf="registerForm.get('confirmPassword')?.errors?.['required']">Please confirm your password</span>
            <span *ngIf="registerForm.errors?.['mismatch']">Passwords do not match</span>
          </div>
        </div>

        <!-- Terms -->
        <div class="flex items-center">
          <input
            id="acceptTerms"
            type="checkbox"
            formControlName="acceptTerms"
            class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
          />
          <label for="acceptTerms" class="ml-2 block text-sm text-gray-900">
            I agree to the
            <a href="#" class="text-indigo-600 hover:text-indigo-500">Terms of Service</a>
            and
            <a href="#" class="text-indigo-600 hover:text-indigo-500">Privacy Policy</a>
          </label>
        </div>
        <div *ngIf="submitted && registerForm.get('acceptTerms')?.invalid" class="text-red-500 text-xs">
          You must accept the terms and conditions
        </div>
      </div>

      <div>
        <button
          type="submit"
          [disabled]="isLoading"
          class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
        >
          <span class="absolute left-0 inset-y-0 flex items-center pl-3">
            <svg *ngIf="!isLoading" class="h-5 w-5 text-indigo-500 group-hover:text-indigo-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
            </svg>
            <svg *ngIf="isLoading" class="animate-spin h-5 w-5 text-indigo-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
          </span>
          {{ isLoading ? 'Creating account...' : 'Create account' }}
        </button>
      </div>

      <div class="text-center">
        <p class="text-sm text-gray-600">
          Already have an account?
          <a routerLink="/login" class="font-medium text-indigo-600 hover:text-indigo-500">
            Sign in here
          </a>
        </p>
      </div>
    </form>
  </div>
</div>
```

```ts
// src/app/shared/components/footer/footer.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent {
  currentYear = new Date().getFullYear();
}
```

```html
<footer class="bg-white border-t border-gray-200 mt-auto">
  <div class="container mx-auto px-4 py-6">
    <div class="flex flex-col md:flex-row justify-between items-center">
      <div class="flex items-center space-x-4">
        <span class="text-sm text-gray-600">
          &copy; {{ currentYear }} Smart Task Management System. All rights reserved.
        </span>
      </div>
      <div class="flex items-center space-x-6 mt-4 md:mt-0">
        <a href="#" class="text-sm text-gray-500 hover:text-gray-700">Privacy Policy</a>
        <a href="#" class="text-sm text-gray-500 hover:text-gray-700">Terms of Service</a>
        <a href="#" class="text-sm text-gray-500 hover:text-gray-700">Contact</a>
      </div>
      <div class="flex items-center space-x-4 mt-4 md:mt-0">
        <span class="text-xs text-gray-400">v1.0.0</span>
      </div>
    </div>
  </div>
</footer>
```

```ts
// src/app/shared/components/notification/notification.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService, Notification } from '../../../core/services/notification.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css']
})
export class NotificationComponent implements OnInit, OnDestroy {
  notifications: (Notification & { id: number })[] = [];
  private subscription: Subscription | null = null;
  private idCounter = 0;

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.subscription = this.notificationService.notifications$.subscribe(notification => {
      const id = this.idCounter++;
      this.notifications.push({ ...notification, id });
      
      if (notification.duration && notification.duration > 0) {
        setTimeout(() => {
          this.removeNotification(id);
        }, notification.duration);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  removeNotification(id: number): void {
    this.notifications = this.notifications.filter(n => n.id !== id);
  }

  getNotificationClasses(type: string): string {
    const baseClasses = 'fixed top-4 right-4 z-50 p-4 rounded-lg shadow-lg max-w-md transform transition-all duration-500';
    const typeClasses = {
      success: 'bg-green-50 border-l-4 border-green-500 text-green-700',
      error: 'bg-red-50 border-l-4 border-red-500 text-red-700',
      warning: 'bg-yellow-50 border-l-4 border-yellow-500 text-yellow-700',
      info: 'bg-blue-50 border-l-4 border-blue-500 text-blue-700'
    };
    return `${baseClasses} ${typeClasses[type as keyof typeof typeClasses] || typeClasses.info}`;
  }

  getIcon(type: string): string {
    const icons = {
      success: '✓',
      error: '✕',
      warning: '⚠',
      info: 'ℹ'
    };
    return icons[type as keyof typeof icons] || icons.info;
  }
}
```

```html
<!-- src/app/shared/components/notification/notification.component.html -->
<div class="fixed top-4 right-4 z-50 space-y-2 max-w-md w-full">
  <div
    *ngFor="let notification of notifications"
    [class]="getNotificationClasses(notification.type)"
    role="alert"
  >
    <div class="flex items-start">
      <div class="flex-shrink-0">
        <span class="text-lg font-bold">{{ getIcon(notification.type) }}</span>
      </div>
      <div class="ml-3 flex-1">
        <p class="text-sm font-medium">{{ notification.message }}</p>
      </div>
      <div class="ml-4 flex-shrink-0">
        <button
          (click)="removeNotification(notification.id)"
          class="text-gray-400 hover:text-gray-600 focus:outline-none"
        >
          <span class="sr-only">Close</span>
          <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
    </div>
  </div>
</div>
```


# Part 11 : Project Management Components / Project Module
## Project Service
```ts
// src/app/core/services/project.service.ts
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
```

## Project List Component
```ts
// src/app/features/projects/project-list/project-list.component.ts
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
```

```html
<!-- src/app/features/projects/project-list/project-list.component.html -->
 <div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
            <h1 class="text-2xl font-bold text-gray-900">Projects</h1>
            <p class="text-sm text-gray-500 mt-1">Manage all your projects</p>
        </div>
        <div *ngIf="isAdminOrManager" class="flex gap-2">
            <button (click)="showFilters = !showFilters"
                class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50">
                {{ showFilters ? 'Hide Filters' : 'Show Filters' }}
            </button>
            <a routerLink="/projects/create"
                class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700">
                + New Project
            </a>
        </div>
    </div>

    <!-- Search Bar -->
    <div class="relative">
        <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
        </div>
        <input type="text" [(ngModel)]="searchTerm" (input)="onSearch($event)"
            placeholder="Search projects by name or description..."
            class="w-full pl-10 pr-10 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent" />
        <button *ngIf="searchTerm" (click)="clearSearch()" class="absolute inset-y-0 right-0 pr-3 flex items-center">
            <svg class="h-5 w-5 text-gray-400 hover:text-gray-600" fill="none" stroke="currentColor"
                viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
        </button>
    </div>

    <!-- Filters -->
    <div *ngIf="showFilters" class="bg-gray-50 p-4 rounded-lg border border-gray-200">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
                <select [(ngModel)]="filter.isActive"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500">
                    <option [ngValue]="undefined">All</option>
                    <option [ngValue]="true">Active</option>
                    <option [ngValue]="false">Inactive</option>
                </select>
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Start Date From</label>
                <input type="date" [(ngModel)]="filter.startDateFrom"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Start Date To</label>
                <input type="date" [(ngModel)]="filter.startDateTo"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            </div>
        </div>
        <div class="flex justify-end gap-2 mt-4">
            <button (click)="resetFilters()"
                class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50">
                Reset
            </button>
            <button (click)="applyFilters()"
                class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700">
                Apply Filters
            </button>
        </div>
    </div>

    <!-- Projects Table -->
    <div class="bg-white rounded-lg border border-gray-200 overflow-hidden">
        <div *ngIf="loading" class="flex justify-center items-center py-12">
            <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
        </div>

        <div *ngIf="!loading && projects.length === 0" class="text-center py-12">
            <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            <h3 class="mt-2 text-sm font-medium text-gray-900">No projects found</h3>
            <p class="mt-1 text-sm text-gray-500">Get started by creating a new project.</p>
            <div *ngIf="isAdminOrManager" class="mt-6">
                <a routerLink="/projects/create"
                    class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700">
                    + New Project
                </a>
            </div>
        </div>

        <div *ngIf="!loading && projects.length > 0" class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
                <thead class="bg-gray-50">
                    <tr>
                        <th (click)="changeSort('name')"
                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100">
                            Project {{ getSortIcon('name') }}
                        </th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Description
                        </th>
                        <th (click)="changeSort('startDate')"
                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100">
                            Start Date {{ getSortIcon('startDate') }}
                        </th>
                        <th (click)="changeSort('taskCount')"
                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100">
                            Tasks {{ getSortIcon('taskCount') }}
                        </th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Progress
                        </th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Status
                        </th>
                        <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Actions
                        </th>
                    </tr>
                </thead>
                <tbody class="bg-white divide-y divide-gray-200">
                    <tr *ngFor="let project of projects" class="hover:bg-gray-50">
                        <td class="px-6 py-4">
                            <div class="flex items-center">
                                <div
                                    class="flex-shrink-0 h-10 w-10 bg-indigo-100 rounded-lg flex items-center justify-center">
                                    <span class="text-indigo-600 font-medium text-sm">
                                        {{ project.name.charAt(0).toUpperCase() }}
                                    </span>
                                </div>
                                <div class="ml-4">
                                    <a [routerLink]="['/projects', project.id]"
                                        class="text-sm font-medium text-gray-900 hover:text-indigo-600">
                                        {{ project.name }}
                                    </a>
                                    <p class="text-xs text-gray-500">Created by {{ project.createdByUser?.username }}
                                    </p>
                                </div>
                            </div>
                        </td>
                        <td class="px-6 py-4">
                            <div class="text-sm text-gray-500 max-w-xs truncate">
                                {{ project.description || 'No description' }}
                            </div>
                        </td>
                        <td class="px-6 py-4 text-sm text-gray-500">
                            {{ project.startDate | date:'MMM d, y' }}
                        </td>
                        <td class="px-6 py-4 text-sm text-gray-500">
                            {{ project.taskCount }}
                            <span class="text-xs text-gray-400">({{ project.completedTasks }} completed)</span>
                        </td>
                        <td class="px-6 py-4">
                            <div class="flex items-center">
                                <div class="w-24 h-2 bg-gray-200 rounded-full overflow-hidden">
                                    <div class="h-full bg-green-500 rounded-full transition-all"
                                        [style.width]="project.taskCount ? (project.completedTasks / project.taskCount * 100) + '%' : '0%'">
                                    </div>
                                </div>
                                <span class="ml-2 text-xs text-gray-500">
                                    {{ project.taskCount ? (project.completedTasks / project.taskCount * 100 |
                                    number:'1.0-0') : '0' }}%
                                </span>
                            </div>
                        </td>
                        <td class="px-6 py-4">
                            <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                                [class]="getStatusBadgeClass(project.isActive)">
                                {{ getStatusText(project.isActive) }}
                            </span>
                        </td>
                        <td class="px-6 py-4 text-right text-sm font-medium">
                            <div class="flex justify-end gap-2">
                                <a [routerLink]="['/projects', project.id]"
                                    class="text-indigo-600 hover:text-indigo-900">
                                    <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                                    </svg>
                                </a>
                                <a *ngIf="isAdminOrManager" [routerLink]="['/projects', project.id, 'edit']"
                                    class="text-blue-600 hover:text-blue-900">
                                    <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                    </svg>
                                </a>
                                <button *ngIf="isAdminOrManager" (click)="toggleProjectStatus(project.id)"
                                    class="text-yellow-600 hover:text-yellow-900">
                                    <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                </button>
                                <button *ngIf="isAdminOrManager" (click)="deleteProject(project.id, project.name)"
                                    class="text-red-600 hover:text-red-900">
                                    <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                    </svg>
                                </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <!-- Pagination -->
    <div *ngIf="!loading && projects.length > 0"
        class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6">
        <div class="flex flex-1 justify-between sm:hidden">
            <button (click)="changePage(currentPage - 1)" [disabled]="currentPage === 1"
                class="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50">
                Previous
            </button>
            <button (click)="changePage(currentPage + 1)" [disabled]="currentPage === totalPages"
                class="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50">
                Next
            </button>
        </div>
        <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
            <div>
                <p class="text-sm text-gray-700">
                    Showing
                    <span class="font-medium">{{ ((currentPage - 1) * pageSize) + 1 }}</span>
                    to
                    <span class="font-medium">{{ Math.min(currentPage * pageSize, totalItems) }}</span>
                    of
                    <span class="font-medium">{{ totalItems }}</span>
                    results
                </p>
            </div>
            <div>
                <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                    <button (click)="changePage(currentPage - 1)" [disabled]="currentPage === 1"
                        class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50">
                        <span class="sr-only">Previous</span>
                        <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                            <path fill-rule="evenodd"
                                d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z"
                                clip-rule="evenodd" />
                        </svg>
                    </button>
                    <button *ngFor="let page of getPageNumbers()" (click)="changePage(page)"
                        [class]="page === currentPage
              ? 'relative z-10 inline-flex items-center bg-indigo-600 px-4 py-2 text-sm font-semibold text-white focus:z-20 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600'
              : 'relative inline-flex items-center px-4 py-2 text-sm font-semibold text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0'">
                        {{ page }}
                    </button>
                    <button (click)="changePage(currentPage + 1)" [disabled]="currentPage === totalPages"
                        class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50">
                        <span class="sr-only">Next</span>
                        <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                            <path fill-rule="evenodd"
                                d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z"
                                clip-rule="evenodd" />
                        </svg>
                    </button>
                </nav>
            </div>
        </div>
    </div>
</div>
```

## Project Form Component
```ts
// src/app/features/projects/project-form/project-form.component.ts
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
```

```html
<!-- src/app/features/projects/project-form/project-form.component.html -->
<div class="max-w-3xl mx-auto">
    <div class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200">
            <h2 class="text-xl font-semibold text-gray-900">
                {{ isEditMode ? 'Edit Project' : 'Create New Project' }}
            </h2>
            <p class="text-sm text-gray-500 mt-1">
                {{ isEditMode ? 'Update project details' : 'Add a new project to the system' }}
            </p>
        </div>

        <form [formGroup]="projectForm" (ngSubmit)="onSubmit()" class="p-6 space-y-6">
            <!-- Project Name -->
            <div>
                <label for="name" class="block text-sm font-medium text-gray-700">
                    Project Name <span class="text-red-500">*</span>
                </label>
                <input id="name" type="text" formControlName="name"
                    class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    placeholder="Enter project name"
                    [class.border-red-500]="projectForm.get('name')?.invalid && projectForm.get('name')?.touched" />
                <div *ngIf="projectForm.get('name')?.invalid && projectForm.get('name')?.touched"
                    class="text-red-500 text-xs mt-1">
                    <span *ngIf="projectForm.get('name')?.errors?.['required']">Project name is required</span>
                    <span *ngIf="projectForm.get('name')?.errors?.['maxlength']">Maximum 100 characters</span>
                </div>
            </div>

            <!-- Description -->
            <div>
                <label for="description" class="block text-sm font-medium text-gray-700">
                    Description
                </label>
                <textarea id="description" rows="3" formControlName="description"
                    class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    placeholder="Describe the project"
                    [class.border-red-500]="projectForm.get('description')?.invalid && projectForm.get('description')?.touched"></textarea>
                <div *ngIf="projectForm.get('description')?.invalid && projectForm.get('description')?.touched"
                    class="text-red-500 text-xs mt-1">
                    <span *ngIf="projectForm.get('description')?.errors?.['maxlength']">Maximum 500 characters</span>
                </div>
            </div>

            <!-- Start and End Dates -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                    <label for="startDate" class="block text-sm font-medium text-gray-700">
                        Start Date <span class="text-red-500">*</span>
                    </label>
                    <input id="startDate" type="date" formControlName="startDate"
                        class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        [class.border-red-500]="projectForm.get('startDate')?.invalid && projectForm.get('startDate')?.touched" />
                    <div *ngIf="projectForm.get('startDate')?.invalid && projectForm.get('startDate')?.touched"
                        class="text-red-500 text-xs mt-1">
                        <span *ngIf="projectForm.get('startDate')?.errors?.['required']">Start date is required</span>
                    </div>
                </div>
                <div>
                    <label for="endDate" class="block text-sm font-medium text-gray-700">
                        End Date
                    </label>
                    <input id="endDate" type="date" formControlName="endDate"
                        class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm" />
                </div>
            </div>

            <!-- Status (only in edit mode) -->
            <div *ngIf="isEditMode" class="flex items-center">
                <input id="isActive" type="checkbox" formControlName="isActive"
                    class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded" />
                <label for="isActive" class="ml-2 block text-sm text-gray-900">
                    Project is active
                </label>
            </div>

            <!-- Form Actions -->
            <div class="flex flex-col sm:flex-row justify-end gap-3 pt-4 border-t border-gray-200">
                <button type="button" (click)="onCancel()"
                    class="w-full sm:w-auto px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
                    Cancel
                </button>
                <button type="submit" [disabled]="isLoading"
                    class="w-full sm:w-auto px-4 py-2 bg-indigo-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50">
                    <span class="flex items-center justify-center">
                        <svg *ngIf="isLoading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                            xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4">
                            </circle>
                            <path class="opacity-75" fill="currentColor"
                                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                            </path>
                        </svg>
                        {{ isLoading ? 'Saving...' : (isEditMode ? 'Update Project' : 'Create Project') }}
                    </span>
                </button>
            </div>
        </form>
    </div>
</div>
```

## Project Detail Component
```ts
// src/app/features/projects/project-detail/project-detail.component.ts
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

```

```html
<!-- src/app/features/projects/project-detail/project-detail.component.html -->
<div *ngIf="loading" class="flex justify-center items-center py-12">
    <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
</div>

<div *ngIf="!loading && project" class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
            <div class="flex items-center gap-3">
                <h1 class="text-2xl font-bold text-gray-900">{{ project.name }}</h1>
                <span class="px-2 py-1 text-xs font-semibold rounded-full"
                    [class]="getStatusBadgeClass(project.isActive)">
                    {{ getStatusText(project.isActive) }}
                </span>
            </div>
            <p class="text-sm text-gray-500 mt-1">
                Created by {{ project.createdByUser?.username }} on {{ project.createdAt | date:'MMM d, y' }}
            </p>
        </div>
        <div class="flex gap-2">
            <a routerLink="/tasks" [queryParams]="{ projectId: project.id }"
                class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50">
                View Tasks
            </a>
            <a *ngIf="isAdminOrManager" [routerLink]="['/projects', project.id, 'edit']"
                class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700">
                Edit Project
            </a>
            <button *ngIf="isAdminOrManager" (click)="toggleStatus()"
                class="px-4 py-2 border border-yellow-300 text-yellow-700 rounded-md text-sm font-medium hover:bg-yellow-50">
                {{ project.isActive ? 'Deactivate' : 'Activate' }}
            </button>
            <button *ngIf="isAdminOrManager" (click)="deleteProject()"
                class="px-4 py-2 border border-red-300 text-red-700 rounded-md text-sm font-medium hover:bg-red-50">
                Delete
            </button>
        </div>
    </div>

    <!-- Description -->
    <div class="bg-white rounded-lg border border-gray-200 p-6">
        <h3 class="text-sm font-medium text-gray-700 mb-2">Description</h3>
        <p class="text-gray-600">{{ project.description || 'No description provided.' }}</p>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div class="bg-white rounded-lg border border-gray-200 p-4">
            <p class="text-sm text-gray-500">Total Tasks</p>
            <p class="text-2xl font-bold text-gray-900">{{ summary?.totalTasks || 0 }}</p>
        </div>
        <div class="bg-white rounded-lg border border-gray-200 p-4">
            <p class="text-sm text-gray-500">Completed</p>
            <p class="text-2xl font-bold text-green-600">{{ summary?.completedTasks || 0 }}</p>
        </div>
        <div class="bg-white rounded-lg border border-gray-200 p-4">
            <p class="text-sm text-gray-500">In Progress</p>
            <p class="text-2xl font-bold text-yellow-600">{{ summary?.inProgressTasks || 0 }}</p>
        </div>
        <div class="bg-white rounded-lg border border-gray-200 p-4">
            <p class="text-sm text-gray-500">Completion</p>
            <p class="text-2xl font-bold text-indigo-600">{{ summary?.completionPercentage || 0 }}%</p>
        </div>
    </div>

    <!-- Progress Bar -->
    <div class="bg-white rounded-lg border border-gray-200 p-6">
        <div class="flex justify-between items-center mb-2">
            <h3 class="text-sm font-medium text-gray-700">Overall Progress</h3>
            <span class="text-sm font-medium text-gray-900">{{ summary?.completionPercentage || 0 }}%</span>
        </div>
        <div class="w-full h-3 bg-gray-200 rounded-full overflow-hidden">
            <div class="h-full rounded-full transition-all duration-500"
                [class]="getProgressColor(summary?.completionPercentage || 0)"
                [style.width]="(summary?.completionPercentage || 0) + '%'"></div>
        </div>
        <div class="flex justify-between text-xs text-gray-500 mt-2">
            <span>0%</span>
            <span>100%</span>
        </div>
    </div>

    <!-- Task Breakdown -->
    <div class="bg-white rounded-lg border border-gray-200 p-6">
        <h3 class="text-sm font-medium text-gray-700 mb-4">Task Breakdown</h3>
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div class="text-center">
                <div class="text-2xl font-bold text-gray-900">{{ summary?.toDoTasks || 0 }}</div>
                <div class="text-xs text-gray-500">To Do</div>
            </div>
            <div class="text-center">
                <div class="text-2xl font-bold text-yellow-600">{{ summary?.inProgressTasks || 0 }}</div>
                <div class="text-xs text-gray-500">In Progress</div>
            </div>
            <div class="text-center">
                <div class="text-2xl font-bold text-green-600">{{ summary?.completedTasks || 0 }}</div>
                <div class="text-xs text-gray-500">Completed</div>
            </div>
            <div class="text-center">
                <div class="text-2xl font-bold text-red-600">{{ summary?.cancelledTasks || 0 }}</div>
                <div class="text-xs text-gray-500">Cancelled</div>
            </div>
        </div>
    </div>

    <!-- Date Information -->
    <div class="bg-white rounded-lg border border-gray-200 p-6">
        <h3 class="text-sm font-medium text-gray-700 mb-2">Timeline</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
                <p class="text-sm text-gray-500">Start Date</p>
                <p class="text-sm font-medium text-gray-900">{{ project.startDate | date:'MMM d, y' }}</p>
            </div>
            <div>
                <p class="text-sm text-gray-500">End Date</p>
                <p class="text-sm font-medium text-gray-900">{{ project.endDate ? (project.endDate | date:'MMM d, y') :
                    'Not set' }}</p>
            </div>
        </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex flex-wrap gap-2">
        <a routerLink="/tasks/create" [queryParams]="{ projectId: project.id }"
            class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700">
            + Create Task
        </a>
        <a routerLink="/projects"
            class="px-4 py-2 border border-gray-300 text-gray-700 rounded-md text-sm font-medium hover:bg-gray-50">
            Back to Projects
        </a>
    </div>
</div>
```

## Update App Module with HttpClient Interceptors
```ts
// src/app/app.config.ts (Update)
import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection  } from '@angular/core';
import { provideRouter, withComponentInputBinding, withRouterConfig } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { routes } from './app.routes';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { ErrorInterceptor } from './core/interceptors/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(
      routes,
      withComponentInputBinding(),
      withRouterConfig({
        paramsInheritanceStrategy: 'always'
      })
    ),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    }
  ]
};

```

# Part 12 : Task Management Components / Task Module
## Task Service

```ts
// src/app/core/services/task.service.ts
// src/app/core/services/task.service.ts

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
    Task,
    CreateTaskRequest,
    UpdateTaskRequest,
    UpdateTaskStatusRequest,
    TaskFilter,
    PagedResponse,
    TaskStatistics
} from '../models/task.model';

@Injectable({
    providedIn: 'root'
})
export class TaskService {
    constructor(private apiService: ApiService) { }

    getTasks(filter: TaskFilter): Observable<PagedResponse<Task>> {
        return this.apiService.get<PagedResponse<Task>>('tasks', filter);
    }

    getTaskById(id: string): Observable<Task> {
        return this.apiService.get<Task>(`tasks/${id}`);
    }

    createTask(request: CreateTaskRequest): Observable<Task> {
        return this.apiService.post<Task>('tasks', request);
    }

    updateTask(id: string, request: UpdateTaskRequest): Observable<Task> {
        return this.apiService.put<Task>(`tasks/${id}`, request);
    }

    deleteTask(id: string): Observable<void> {
        return this.apiService.delete<void>(`tasks/${id}`);
    }

    updateTaskStatus(id: string, request: UpdateTaskStatusRequest): Observable<Task> {
        return this.apiService.patch<Task>(`tasks/${id}/status`, request);
    }

    assignTask(id: string, userId: string): Observable<Task> {
        return this.apiService.patch<Task>(`tasks/${id}/assign`, { userId });
    }

    getTaskStatistics(projectId?: string, userId?: string): Observable<TaskStatistics> {
        const params: any = {};
        if (projectId) params.projectId = projectId;
        if (userId) params.userId = userId;
        return this.apiService.get<TaskStatistics>('tasks/statistics', params);
    }

    getOverdueTasks(): Observable<Task[]> {
        return this.apiService.get<Task[]>('tasks/overdue');
    }

    getTasksDueSoon(daysThreshold: number = 7): Observable<Task[]> {
        return this.apiService.get<Task[]>('tasks/due-soon', { daysThreshold });
    }

    bulkUpdateStatus(taskIds: string[], newStatus: number): Observable<void> {
        return this.apiService.patch<void>('tasks/bulk-status', { taskIds, newStatus });
    }
}
```

## AI Service for Task Improvement
```ts
// src/app/core/services/ai.service.ts
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
}
```

## Task List Component
```ts
// src/app/features/tasks/task-list/task-list.component.ts
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
```

```html
<!-- src/app/features/tasks/task-list/task-list.component.html -->
<div class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
            <h1 class="text-2xl font-bold text-gray-900">Tasks</h1>
            <p class="text-sm text-gray-500 mt-1">Manage all your tasks</p>
        </div>
        <div class="flex gap-2">
            <button (click)="showFilters = !showFilters"
                class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50">
                {{ showFilters ? 'Hide Filters' : 'Show Filters' }}
            </button>
            <a routerLink="/tasks/create" [queryParams]="projectId ? { projectId: projectId } : {}"
                class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700">
                + New Task
            </a>
        </div>
    </div>

    <!-- Search Bar -->
    <div class="relative">
        <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
        </div>
        <input type="text" [(ngModel)]="searchTerm" (input)="onSearch($event)"
            placeholder="Search tasks by title or description..."
            class="w-full pl-10 pr-10 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent" />
        <button *ngIf="searchTerm" (click)="clearSearch()" class="absolute inset-y-0 right-0 pr-3 flex items-center">
            <svg class="h-5 w-5 text-gray-400 hover:text-gray-600" fill="none" stroke="currentColor"
                viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
        </button>
    </div>

    <!-- Filters -->
    <div *ngIf="showFilters" class="bg-gray-50 p-4 rounded-lg border border-gray-200">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
                <select [(ngModel)]="filter.status"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500">
                    <option [ngValue]="undefined">All</option>
                    <option *ngFor="let status of taskStatuses" [ngValue]="status">
                        {{ statusLabels[status] }}
                    </option>
                </select>
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Priority</label>
                <select [(ngModel)]="filter.priority"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500">
                    <option [ngValue]="undefined">All</option>
                    <option *ngFor="let priority of taskPriorities" [ngValue]="priority">
                        {{ priorityLabels[priority] }}
                    </option>
                </select>
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Overdue</label>
                <select [(ngModel)]="filter.isOverdue"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500">
                    <option [ngValue]="undefined">All</option>
                    <option [ngValue]="true">Overdue Only</option>
                    <option [ngValue]="false">Not Overdue</option>
                </select>
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Due Date From</label>
                <input type="date" [(ngModel)]="filter.dueDateFrom"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Due Date To</label>
                <input type="date" [(ngModel)]="filter.dueDateTo"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500" />
            </div>
            <div class="flex items-center">
                <label class="flex items-center space-x-2 text-sm text-gray-700">
                    <input type="checkbox" [(ngModel)]="filter.showOnlyAssignedToMe"
                        class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded" />
                    <span>Show only assigned to me</span>
                </label>
            </div>
        </div>
        <div class="flex justify-end gap-2 mt-4">
            <button (click)="resetFilters()"
                class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50">
                Reset
            </button>
            <button (click)="applyFilters()"
                class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700">
                Apply Filters
            </button>
        </div>
    </div>

    <!-- Bulk Actions -->
    <div *ngIf="showBulkActions"
        class="bg-indigo-50 border border-indigo-200 rounded-lg p-4 flex flex-wrap items-center gap-4">
        <span class="text-sm font-medium text-indigo-700">
            {{ selectedTasks.size }} task(s) selected
        </span>
        <div class="flex items-center gap-2">
            <select [(ngModel)]="bulkStatus"
                class="px-3 py-1 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500">
                <option [ngValue]="null">Change status to...</option>
                <option *ngFor="let status of taskStatuses" [ngValue]="status">
                    {{ statusLabels[status] }}
                </option>
            </select>
            <button (click)="applyBulkStatus()" [disabled]="!bulkStatus"
                class="px-3 py-1 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700 disabled:opacity-50">
                Apply
            </button>
        </div>
        <button (click)="selectedTasks.clear(); showBulkActions = false"
            class="text-sm text-gray-500 hover:text-gray-700">
            Clear selection
        </button>
    </div>

    <!-- Tasks Table -->
    <div class="bg-white rounded-lg border border-gray-200 overflow-hidden">
        <div *ngIf="loading" class="flex justify-center items-center py-12">
            <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
        </div>

        <div *ngIf="!loading && tasks.length === 0" class="text-center py-12">
            <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
            </svg>
            <h3 class="mt-2 text-sm font-medium text-gray-900">No tasks found</h3>
            <p class="mt-1 text-sm text-gray-500">Get started by creating a new task.</p>
            <div class="mt-6">
                <a routerLink="/tasks/create" [queryParams]="projectId ? { projectId: projectId } : {}"
                    class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700">
                    + New Task
                </a>
            </div>
        </div>

        <div *ngIf="!loading && tasks.length > 0" class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
                <thead class="bg-gray-50">
                    <tr>
                        <th class="px-6 py-3 text-left">
                            <input type="checkbox" (change)="selectAllTasks()"
                                [checked]="selectedTasks.size === tasks.length && tasks.length > 0"
                                class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded" />
                        </th>
                        <th (click)="changeSort('title')"
                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100">
                            Task {{ getSortIcon('title') }}
                        </th>
                        <th (click)="changeSort('status')"
                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100">
                            Status {{ getSortIcon('status') }}
                        </th>
                        <th (click)="changeSort('priority')"
                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100">
                            Priority {{ getSortIcon('priority') }}
                        </th>
                        <th (click)="changeSort('dueDate')"
                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100">
                            Due Date {{ getSortIcon('dueDate') }}
                        </th>
                        <th (click)="changeSort('projectName')"
                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100">
                            Project {{ getSortIcon('projectName') }}
                        </th>
                        <th (click)="changeSort('assignedto')"
                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100">
                            Assigned To {{ getSortIcon('assignedto') }}
                        </th>
                        <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Actions
                        </th>
                    </tr>
                </thead>
                <tbody class="bg-white divide-y divide-gray-200">
                    <tr *ngFor="let task of tasks" class="hover:bg-gray-50"
                        [class.bg-red-50]="task.isOverdue && task.status !== 2 && task.status !== 3">
                        <td class="px-6 py-4">
                            <input type="checkbox" [checked]="selectedTasks.has(task.id)"
                                (change)="toggleTaskSelection(task.id)"
                                class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded" />
                        </td>
                        <td class="px-6 py-4">
                            <div>
                                <a [routerLink]="['/tasks', task.id]"
                                    class="text-sm font-medium text-gray-900 hover:text-indigo-600">
                                    {{ task.title }}
                                </a>
                                <p class="text-xs text-gray-500 truncate max-w-xs">{{ task.description }}</p>
                                <div *ngIf="task.isOverdue && task.status !== 2 && task.status !== 3" class="mt-1">
                                    <span class="text-xs text-red-600 font-medium">Overdue</span>
                                </div>
                            </div>
                        </td>
                        <td class="px-6 py-4">
                            <span class="px-2 py-1 text-xs font-semibold rounded-full"
                                [class]="getStatusBadgeClass(task.status)">
                                {{ getStatusText(task.status) }}
                            </span>
                            <div class="mt-1">
                                <select (change)="updateTaskStatus(task.id, $event.target.value)"
                                    class="text-xs border border-gray-300 rounded px-1 py-0.5 focus:outline-none focus:ring-1 focus:ring-indigo-500">
                                    <option *ngFor="let status of taskStatuses" [value]="status"
                                        [selected]="status === task.status">
                                        {{ statusLabels[status] }}
                                    </option>
                                </select>
                            </div>
                        </td>
                        <td class="px-6 py-4">
                            <span class="px-2 py-1 text-xs font-semibold rounded-full"
                                [class]="getPriorityBadgeClass(task.priority)">
                                {{ getPriorityText(task.priority) }}
                            </span>
                        </td>
                        <td class="px-6 py-4">
                            <span class="text-sm" [class]="getDueDateClass(task.dueDate)">
                                {{ task.dueDate | date:'MMM d, y' }}
                            </span>
                            <span *ngIf="task.daysUntilDue !== undefined" class="text-xs text-gray-400 block">
                                {{ task.daysUntilDue > 0 ? task.daysUntilDue + ' days left' : task.daysUntilDue === 0 ?
                                'Today' : task.daysUntilDue + ' days overdue' }}
                            </span>
                        </td>
                        <td class="px-6 py-4">
                            <a [routerLink]="['/projects', task.projectId]"
                                class="text-sm text-indigo-600 hover:text-indigo-900">
                                {{ task.projectName }}
                            </a>
                        </td>
                        <td class="px-6 py-4 text-sm text-gray-500">
                            {{ task.assignedToUser?.username || 'Unassigned' }}
                        </td>
                        <td class="px-6 py-4 text-right text-sm font-medium">
                            <div class="flex justify-end gap-2">
                                <a [routerLink]="['/tasks', task.id]" class="text-indigo-600 hover:text-indigo-900">
                                    <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                                    </svg>
                                </a>
                                <a [routerLink]="['/tasks', task.id, 'edit']" class="text-blue-600 hover:text-blue-900">
                                    <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                    </svg>
                                </a>
                                <button (click)="deleteTask(task.id, task.title)"
                                    class="text-red-600 hover:text-red-900">
                                    <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                    </svg>
                                </button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <!-- Pagination -->
    <div *ngIf="!loading && tasks.length > 0"
        class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6">
        <div class="flex flex-1 justify-between sm:hidden">
            <button (click)="changePage(currentPage - 1)" [disabled]="currentPage === 1"
                class="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50">
                Previous
            </button>
            <button (click)="changePage(currentPage + 1)" [disabled]="currentPage === totalPages"
                class="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50">
                Next
            </button>
        </div>
        <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
            <div>
                <p class="text-sm text-gray-700">
                    Showing
                    <span class="font-medium">{{ ((currentPage - 1) * pageSize) + 1 }}</span>
                    to
                    <span class="font-medium">{{ Math.min(currentPage * pageSize, totalItems) }}</span>
                    of
                    <span class="font-medium">{{ totalItems }}</span>
                    results
                </p>
            </div>
            <div>
                <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                    <button (click)="changePage(currentPage - 1)" [disabled]="currentPage === 1"
                        class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50">
                        <span class="sr-only">Previous</span>
                        <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                            <path fill-rule="evenodd"
                                d="M12.79 5.23a.75.75 0 01-.02 1.06L8.832 10l3.938 3.71a.75.75 0 11-1.04 1.08l-4.5-4.25a.75.75 0 010-1.08l4.5-4.25a.75.75 0 011.06.02z"
                                clip-rule="evenodd" />
                        </svg>
                    </button>
                    <button *ngFor="let page of getPageNumbers()" (click)="changePage(page)"
                        [class]="page === currentPage
              ? 'relative z-10 inline-flex items-center bg-indigo-600 px-4 py-2 text-sm font-semibold text-white focus:z-20 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600'
              : 'relative inline-flex items-center px-4 py-2 text-sm font-semibold text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0'">
                        {{ page }}
                    </button>
                    <button (click)="changePage(currentPage + 1)" [disabled]="currentPage === totalPages"
                        class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50">
                        <span class="sr-only">Next</span>
                        <svg class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                            <path fill-rule="evenodd"
                                d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z"
                                clip-rule="evenodd" />
                        </svg>
                    </button>
                </nav>
            </div>
        </div>
    </div>
</div>
```

## Task Form Component

```ts
// src/app/features/tasks/task-form/task-form.component.ts
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
```

```html
<!-- src/app/features/tasks/task-form/task-form.component.html -->
 <div class="max-w-3xl mx-auto">
    <div class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200">
            <h2 class="text-xl font-semibold text-gray-900">
                {{ isEditMode ? 'Edit Task' : 'Create New Task' }}
            </h2>
            <p class="text-sm text-gray-500 mt-1">
                {{ isEditMode ? 'Update task details' : 'Add a new task to the system' }}
            </p>
        </div>

        <form [formGroup]="taskForm" (ngSubmit)="onSubmit()" class="p-6 space-y-6">
            <!-- Title -->
            <div>
                <label for="title" class="block text-sm font-medium text-gray-700">
                    Task Title <span class="text-red-500">*</span>
                </label>
                <input id="title" type="text" formControlName="title"
                    class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    placeholder="Enter task title"
                    [class.border-red-500]="taskForm.get('title')?.invalid && taskForm.get('title')?.touched" />
                <div *ngIf="taskForm.get('title')?.invalid && taskForm.get('title')?.touched"
                    class="text-red-500 text-xs mt-1">
                    <span *ngIf="taskForm.get('title')?.errors?.['required']">Task title is required</span>
                    <span *ngIf="taskForm.get('title')?.errors?.['maxlength']">Maximum 200 characters</span>
                </div>
            </div>

            <!-- Description -->
            <div>
                <label for="description" class="block text-sm font-medium text-gray-700">
                    Description
                </label>
                <textarea id="description" rows="4" formControlName="description"
                    class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    placeholder="Describe the task in detail"
                    [class.border-red-500]="taskForm.get('description')?.invalid && taskForm.get('description')?.touched"></textarea>
                <div *ngIf="taskForm.get('description')?.invalid && taskForm.get('description')?.touched"
                    class="text-red-500 text-xs mt-1">
                    <span *ngIf="taskForm.get('description')?.errors?.['maxlength']">Maximum 1000 characters</span>
                </div>
            </div>

            <!-- Priority and Estimated Hours -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                    <label for="priority" class="block text-sm font-medium text-gray-700">
                        Priority <span class="text-red-500">*</span>
                    </label>
                    <select id="priority" formControlName="priority"
                        class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                        <option *ngFor="let priority of taskPriorities" [value]="priority">
                            {{ priorityLabels[priority] }}
                        </option>
                    </select>
                </div>
                <div>
                    <label for="estimatedHours" class="block text-sm font-medium text-gray-700">
                        Estimated Hours
                    </label>
                    <input id="estimatedHours" type="number" formControlName="estimatedHours"
                        class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                        [class.border-red-500]="taskForm.get('estimatedHours')?.invalid && taskForm.get('estimatedHours')?.touched" />
                    <div *ngIf="taskForm.get('estimatedHours')?.invalid && taskForm.get('estimatedHours')?.touched"
                        class="text-red-500 text-xs mt-1">
                        <span *ngIf="taskForm.get('estimatedHours')?.errors?.['min']">Must be 0 or greater</span>
                        <span *ngIf="taskForm.get('estimatedHours')?.errors?.['max']">Maximum 999 hours</span>
                    </div>
                </div>
            </div>

            <!-- Due Date -->
            <div>
                <label for="dueDate" class="block text-sm font-medium text-gray-700">
                    Due Date <span class="text-red-500">*</span>
                </label>
                <input id="dueDate" type="date" formControlName="dueDate"
                    class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    [class.border-red-500]="taskForm.get('dueDate')?.invalid && taskForm.get('dueDate')?.touched" />
                <div *ngIf="taskForm.get('dueDate')?.invalid && taskForm.get('dueDate')?.touched"
                    class="text-red-500 text-xs mt-1">
                    <span *ngIf="taskForm.get('dueDate')?.errors?.['required']">Due date is required</span>
                </div>
            </div>

            <!-- Project -->
            <div>
                <label for="projectId" class="block text-sm font-medium text-gray-700">
                    Project <span class="text-red-500">*</span>
                </label>
                <select id="projectId" formControlName="projectId"
                    class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    [class.border-red-500]="taskForm.get('projectId')?.invalid && taskForm.get('projectId')?.touched">
                    <option value="">Select a project</option>
                    <option *ngFor="let project of projects" [value]="project.id">
                        {{ project.name }}
                    </option>
                </select>
                <div *ngIf="taskForm.get('projectId')?.invalid && taskForm.get('projectId')?.touched"
                    class="text-red-500 text-xs mt-1">
                    <span *ngIf="taskForm.get('projectId')?.errors?.['required']">Project is required</span>
                </div>
            </div>

            <!-- Assigned To -->
            <div>
                <label for="assignedToUserId" class="block text-sm font-medium text-gray-700">
                    Assign To
                </label>
                <select id="assignedToUserId" formControlName="assignedToUserId"
                    class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                    <option [ngValue]="null">Unassigned</option>
                    <option *ngFor="let user of users" [value]="user.id">
                        {{ user.firstName }} {{ user.lastName }} ({{ user.username }})
                    </option>
                </select>
            </div>

            <!-- Form Actions -->
            <div class="flex flex-col sm:flex-row justify-end gap-3 pt-4 border-t border-gray-200">
                <button type="button" (click)="onCancel()"
                    class="w-full sm:w-auto px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
                    Cancel
                </button>
                <button type="submit" [disabled]="isLoading"
                    class="w-full sm:w-auto px-4 py-2 bg-indigo-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50">
                    <span class="flex items-center justify-center">
                        <svg *ngIf="isLoading" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                            xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4">
                            </circle>
                            <path class="opacity-75" fill="currentColor"
                                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                            </path>
                        </svg>
                        {{ isLoading ? 'Saving...' : (isEditMode ? 'Update Task' : 'Create Task') }}
                    </span>
                </button>
            </div>
        </form>
    </div>
</div>
```

## Task Detail Component
```ts
// src/app/features/tasks/task-detail/task-detail.component.ts
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
```

```html
<!-- src/app/features/tasks/task-detail/task-detail.component.html -->

<div *ngIf="loading" class="flex justify-center items-center py-12">
    <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
</div>

<div *ngIf="!loading && task" class="space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
            <div class="flex items-center gap-3 flex-wrap">
                <h1 class="text-2xl font-bold text-gray-900">{{ task.title }}</h1>
                <span class="px-2 py-1 text-xs font-semibold rounded-full" [class]="getStatusBadgeClass(task.status)">
                    {{ getStatusText(task.status) }}
                </span>
                <span class="px-2 py-1 text-xs font-semibold rounded-full"
                    [class]="getPriorityBadgeClass(task.priority)">
                    {{ getPriorityText(task.priority) }}
                </span>
                <span *ngIf="task.isOverdue && task.status !== 2 && task.status !== 3"
                    class="px-2 py-1 text-xs font-semibold rounded-full bg-red-100 text-red-800">
                    Overdue
                </span>
            </div>
            <p class="text-sm text-gray-500 mt-1">
                Created by {{ task.createdByUser?.username }} on {{ task.createdAt | date:'MMM d, y' }}
                <span *ngIf="task.assignedToUser"> • Assigned to {{ task.assignedToUser.username }}</span>
            </p>
        </div>
        <div class="flex gap-2 flex-wrap">
            <a [routerLink]="['/tasks', task.id, 'edit']" *ngIf="canEdit"
                class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700">
                Edit Task
            </a>
            <button (click)="deleteTask()" *ngIf="isAdminOrManager"
                class="px-4 py-2 border border-red-300 text-red-700 rounded-md text-sm font-medium hover:bg-red-50">
                Delete
            </button>
        </div>
    </div>

    <!-- Status Update -->
    <div class="bg-white rounded-lg border border-gray-200 p-6">
        <h3 class="text-sm font-medium text-gray-700 mb-3">Update Status</h3>
        <div class="flex flex-wrap gap-2">
            <button *ngFor="let status of [0,1,2,3]" (click)="updateStatus(status)"
                class="px-4 py-2 rounded-md text-sm font-medium transition-colors" [class]="status === task.status
          ? 'bg-indigo-600 text-white'
          : 'border border-gray-300 text-gray-700 hover:bg-gray-50'">
                {{ statusLabels[status] }}
            </button>
        </div>
    </div>

    <!-- Description -->
    <div class="bg-white rounded-lg border border-gray-200 p-6">
        <h3 class="text-sm font-medium text-gray-700 mb-2">Description</h3>
        <p class="text-gray-600 whitespace-pre-wrap">{{ task.description || 'No description provided.' }}</p>
    </div>

    <!-- AI Improvement -->
    <div class="bg-white rounded-lg border border-gray-200 p-6">
        <div class="flex justify-between items-center mb-3">
            <h3 class="text-sm font-medium text-gray-700">AI Improvement</h3>
            <button (click)="improveWithAI()" [disabled]="isImproving"
                class="px-4 py-2 bg-purple-600 text-white rounded-md text-sm font-medium hover:bg-purple-700 disabled:opacity-50">
                <span class="flex items-center">
                    <svg *ngIf="!isImproving" class="w-4 h-4 mr-2" fill="none" stroke="currentColor"
                        viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M13 10V3L4 14h7v7l9-11h-7z" />
                    </svg>
                    <svg *ngIf="isImproving" class="animate-spin w-4 h-4 mr-2" xmlns="http://www.w3.org/2000/svg"
                        fill="none" viewBox="0 0 24 24">
                        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4">
                        </circle>
                        <path class="opacity-75" fill="currentColor"
                            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                        </path>
                    </svg>
                    {{ isImproving ? 'Improving...' : 'Improve with AI' }}
                </span>
            </button>
        </div>

        <!-- AI Improvement Result -->
        <div *ngIf="showAIImprovement && improvementResult"
            class="mt-4 border border-purple-200 rounded-lg p-4 bg-purple-50">
            <h4 class="text-sm font-medium text-purple-900 mb-2">AI Suggested Improvement</h4>

            <div class="mb-3">
                <p class="text-xs font-medium text-gray-500">Improved Title</p>
                <p class="text-sm text-gray-900">{{ improvementResult.improvedTitle }}</p>
            </div>

            <div class="mb-3">
                <p class="text-xs font-medium text-gray-500">Improved Description</p>
                <p class="text-sm text-gray-900 whitespace-pre-wrap">{{ improvementResult.improvedDescription }}</p>
            </div>

            <div *ngIf="improvementResult.keyPoints?.length" class="mb-3">
                <p class="text-xs font-medium text-gray-500">Key Points</p>
                <ul class="list-disc list-inside text-sm text-gray-700">
                    <li *ngFor="let point of improvementResult.keyPoints">{{ point }}</li>
                </ul>
            </div>

            <div *ngIf="improvementResult.suggestedActions?.length" class="mb-3">
                <p class="text-xs font-medium text-gray-500">Suggested Actions</p>
                <ul class="list-disc list-inside text-sm text-gray-700">
                    <li *ngFor="let action of improvementResult.suggestedActions">{{ action }}</li>
                </ul>
            </div>

            <div class="flex gap-2 mt-4">
                <button (click)="applyAIImprovement()"
                    class="px-4 py-2 bg-purple-600 text-white rounded-md text-sm font-medium hover:bg-purple-700">
                    Apply Changes
                </button>
                <button (click)="dismissAIImprovement()"
                    class="px-4 py-2 border border-gray-300 text-gray-700 rounded-md text-sm font-medium hover:bg-gray-50">
                    Dismiss
                </button>
            </div>
        </div>
    </div>

    <!-- Task Details -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div class="bg-white rounded-lg border border-gray-200 p-6">
            <h3 class="text-sm font-medium text-gray-700 mb-3">Task Details</h3>
            <dl class="space-y-2">
                <div class="flex justify-between">
                    <dt class="text-sm text-gray-500">Project</dt>
                    <dd class="text-sm font-medium text-gray-900">
                        <a [routerLink]="['/projects', task.projectId]" class="text-indigo-600 hover:text-indigo-900">
                            {{ task.projectName }}
                        </a>
                    </dd>
                </div>
                <div class="flex justify-between">
                    <dt class="text-sm text-gray-500">Due Date</dt>
                    <dd class="text-sm font-medium"
                        [class.text-red-600]="task.isOverdue && task.status !== 2 && task.status !== 3">
                        {{ task.dueDate | date:'MMM d, y' }}
                        <span *ngIf="task.daysUntilDue !== undefined" class="text-xs text-gray-400 block text-right">
                            {{ task.daysUntilDue > 0 ? task.daysUntilDue + ' days left' : task.daysUntilDue === 0 ?
                            'Today' : task.daysUntilDue + ' days overdue' }}
                        </span>
                    </dd>
                </div>
                <div class="flex justify-between">
                    <dt class="text-sm text-gray-500">Estimated Hours</dt>
                    <dd class="text-sm font-medium text-gray-900">{{ task.estimatedHours }}h</dd>
                </div>
                <div class="flex justify-between">
                    <dt class="text-sm text-gray-500">Actual Hours</dt>
                    <dd class="text-sm font-medium text-gray-900">{{ task.actualHours }}h</dd>
                </div>
                <div class="flex justify-between">
                    <dt class="text-sm text-gray-500">Created At</dt>
                    <dd class="text-sm font-medium text-gray-900">{{ task.createdAt | date:'MMM d, y h:mm a' }}</dd>
                </div>
                <div class="flex justify-between">
                    <dt class="text-sm text-gray-500">Updated At</dt>
                    <dd class="text-sm font-medium text-gray-900">{{ task.updatedAt | date:'MMM d, y h:mm a' }}</dd>
                </div>
            </dl>
        </div>

        <div class="bg-white rounded-lg border border-gray-200 p-6">
            <h3 class="text-sm font-medium text-gray-700 mb-3">Assigned To</h3>
            <div *ngIf="task.assignedToUser; else unassigned" class="flex items-center space-x-3">
                <div class="w-10 h-10 rounded-full bg-indigo-100 flex items-center justify-center">
                    <span class="text-indigo-600 font-medium">
                        {{ task.assignedToUser.firstName[0] }}{{ task.assignedToUser.lastName[0] }}
                    </span>
                </div>
                <div>
                    <p class="text-sm font-medium text-gray-900">
                        {{ task.assignedToUser.firstName }} {{ task.assignedToUser.lastName }}
                    </p>
                    <p class="text-xs text-gray-500">{{ task.assignedToUser.username }}</p>
                    <p class="text-xs text-gray-500">{{ task.assignedToUser.role }}</p>
                </div>
            </div>
            <ng-template #unassigned>
                <p class="text-sm text-gray-500">No user assigned</p>
            </ng-template>
        </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex flex-wrap gap-2">
        <a routerLink="/tasks"
            class="px-4 py-2 border border-gray-300 text-gray-700 rounded-md text-sm font-medium hover:bg-gray-50">
            Back to Tasks
        </a>
        <a [routerLink]="['/tasks', task.id, 'edit']" *ngIf="canEdit"
            class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700">
            Edit Task
        </a>
    </div>
</div>
```

# Part 13 : Dashboard Module
## Dashboard Component

```ts
// src/app/features/dashboard/dashboard.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DashboardService } from '../../core/services/dashboard.service';
import { TaskService } from '../../core/services/task.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { 
  DashboardSummary, 
  ProjectProgress, 
  TaskStatistics,
  RecentActivity,
  UserPerformance,
  PerformanceMetrics,
  TaskStatus,
  TaskPriority
} from '../../core/models/dashboard.model';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  // Dashboard Data
  dashboardSummary: DashboardSummary | null = null;
  loading = true;
  loadingTasks = false;
  
  // Filters
  daysToShow = 30;
  selectedProjectId: string | null = null;
  
  // Task Statistics
  taskStatistics: TaskStatistics | null = null;
  recentActivities: RecentActivity[] = [];
  topPerformers: UserPerformance[] = [];
  performanceMetrics: PerformanceMetrics | null = null;
  projectProgress: ProjectProgress[] = [];
  
  // UI State
  activeTab: 'overview' | 'tasks' | 'team' | 'metrics' = 'overview';
  isAdminOrManager = false;
  
  // Colors for charts
  statusColors = {
    [TaskStatus.ToDo]: '#9CA3AF',
    [TaskStatus.InProgress]: '#FBBF24',
    [TaskStatus.Completed]: '#34D399',
    [TaskStatus.Cancelled]: '#F87171'
  };
  
  priorityColors = {
    [TaskPriority.Low]: '#60A5FA',
    [TaskPriority.Medium]: '#34D399',
    [TaskPriority.High]: '#FB923C',
    [TaskPriority.Critical]: '#F87171'
  };

  private destroy$ = new Subject<void>();

  constructor(
    private dashboardService: DashboardService,
    private taskService: TaskService,
    private authService: AuthService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.isAdminOrManager = this.authService.isAdmin() || this.authService.isProjectManager();
    this.loadDashboard();
    this.loadTaskStatistics();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDashboard(): void {
    this.loading = true;
    const filter = {
      daysToShow: this.daysToShow,
      projectId: this.selectedProjectId || undefined
    };

    this.dashboardService.getDashboardSummary(filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (summary) => {
          this.dashboardSummary = summary;
          this.projectProgress = summary.projectStatistics.projectProgress;
          this.recentActivities = summary.recentActivity.recentActivities;
          this.topPerformers = summary.userStatistics.topPerformers;
          this.performanceMetrics = summary.performanceMetrics;
          this.loading = false;
        },
        error: () => {
          this.notificationService.error('Failed to load dashboard');
          this.loading = false;
        }
      });
  }

  loadTaskStatistics(): void {
    this.loadingTasks = true;
    this.taskService.getTaskStatistics(
      this.selectedProjectId || undefined,
      undefined
    ).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (stats) => {
          this.taskStatistics = stats;
          this.loadingTasks = false;
        },
        error: () => {
          this.loadingTasks = false;
        }
      });
  }

  refreshData(): void {
    this.loadDashboard();
    this.loadTaskStatistics();
  }

  changeTab(tab: 'overview' | 'tasks' | 'team' | 'metrics'): void {
    this.activeTab = tab;
  }

  getStatusLabel(status: number): string {
    const labels = ['To Do', 'In Progress', 'Completed', 'Cancelled'];
    return labels[status] || 'Unknown';
  }

  getPriorityLabel(priority: number): string {
    const labels = ['Low', 'Medium', 'High', 'Critical'];
    return labels[priority] || 'Unknown';
  }

  getStatusColor(status: number): string {
    return this.statusColors[status as TaskStatus] || '#9CA3AF';
  }

  getPriorityColor(priority: number): string {
    return this.priorityColors[priority as TaskPriority] || '#9CA3AF';
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

  getStatusBadgeClass(status: number): string {
    const classes = {
      [TaskStatus.ToDo]: 'bg-gray-100 text-gray-800',
      [TaskStatus.InProgress]: 'bg-yellow-100 text-yellow-800',
      [TaskStatus.Completed]: 'bg-green-100 text-green-800',
      [TaskStatus.Cancelled]: 'bg-red-100 text-red-800'
    };
    return classes[status as TaskStatus] || classes[TaskStatus.ToDo];
  }

  getPriorityBadgeClass(priority: number): string {
    const classes = {
      [TaskPriority.Low]: 'bg-blue-100 text-blue-800',
      [TaskPriority.Medium]: 'bg-green-100 text-green-800',
      [TaskPriority.High]: 'bg-orange-100 text-orange-800',
      [TaskPriority.Critical]: 'bg-red-100 text-red-800'
    };
    return classes[priority as TaskPriority] || classes[TaskPriority.Low];
  }

  // Helper for template iteration
  getKeys(obj: any): string[] {
    return Object.keys(obj);
  }

  getValues(obj: any): number[] {
    return Object.values(obj);
  }

  Math = Math;
  Object = Object;
}
```

```html
<!-- src/app/features/dashboard/dashboard.component.html -->
<div class="space-y-6">
  <!-- Header -->
  <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
    <div>
      <h1 class="text-2xl font-bold text-gray-900">Dashboard</h1>
      <p class="text-sm text-gray-500 mt-1">Overview of your projects and tasks</p>
    </div>
    <div class="flex gap-2">
      <select
        [(ngModel)]="daysToShow"
        (change)="refreshData()"
        class="px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
      >
        <option value="7">Last 7 days</option>
        <option value="14">Last 14 days</option>
        <option value="30">Last 30 days</option>
        <option value="90">Last 90 days</option>
      </select>
      <button
        (click)="refreshData()"
        class="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm font-medium hover:bg-indigo-700"
      >
        Refresh
      </button>
    </div>
  </div>

  <!-- Loading State -->
  <div *ngIf="loading" class="flex justify-center items-center py-12">
    <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
  </div>

  <!-- Dashboard Content -->
  <div *ngIf="!loading && dashboardSummary">
    <!-- Quick Stats -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="bg-white rounded-lg border border-gray-200 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-500">Total Projects</p>
            <p class="text-2xl font-bold text-gray-900">{{ dashboardSummary.projectStatistics.totalProjects }}</p>
          </div>
          <div class="p-3 bg-indigo-100 rounded-full">
            <svg class="w-6 h-6 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
            </svg>
          </div>
        </div>
        <div class="mt-2 flex items-center text-sm">
          <span class="text-green-600">{{ dashboardSummary.projectStatistics.activeProjects }} active</span>
          <span class="mx-2 text-gray-300">|</span>
          <span class="text-gray-500">{{ dashboardSummary.projectStatistics.projectCompletionRate }}% completed</span>
        </div>
      </div>

      <div class="bg-white rounded-lg border border-gray-200 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-500">Total Tasks</p>
            <p class="text-2xl font-bold text-gray-900">{{ dashboardSummary.taskStatistics.totalTasks }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-full">
            <svg class="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
            </svg>
          </div>
        </div>
        <div class="mt-2 flex items-center text-sm">
          <span class="text-green-600">{{ dashboardSummary.taskStatistics.completedTasks }} completed</span>
          <span class="mx-2 text-gray-300">|</span>
          <span class="text-gray-500">{{ dashboardSummary.taskStatistics.completionRate }}% rate</span>
        </div>
      </div>

      <div class="bg-white rounded-lg border border-gray-200 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-500">In Progress</p>
            <p class="text-2xl font-bold text-yellow-600">{{ dashboardSummary.taskStatistics.inProgressTasks }}</p>
          </div>
          <div class="p-3 bg-yellow-100 rounded-full">
            <svg class="w-6 h-6 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
        </div>
        <div class="mt-2 flex items-center text-sm">
          <span class="text-red-600">{{ dashboardSummary.taskStatistics.overdueTasks }} overdue</span>
          <span class="mx-2 text-gray-300">|</span>
          <span class="text-gray-500">{{ dashboardSummary.taskStatistics.tasksDueThisWeek }} due this week</span>
        </div>
      </div>

      <div class="bg-white rounded-lg border border-gray-200 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-500">Team Members</p>
            <p class="text-2xl font-bold text-gray-900">{{ dashboardSummary.userStatistics.totalUsers }}</p>
          </div>
          <div class="p-3 bg-purple-100 rounded-full">
            <svg class="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
          </div>
        </div>
        <div class="mt-2 flex items-center text-sm">
          <span class="text-green-600">{{ dashboardSummary.userStatistics.activeUsers }} active</span>
          <span class="mx-2 text-gray-300">|</span>
          <span class="text-gray-500">{{ dashboardSummary.userStatistics.usersByRole['Admin'] || 0 }} admins</span>
        </div>
      </div>
    </div>

    <!-- Tabs -->
    <div class="border-b border-gray-200">
      <nav class="-mb-px flex space-x-8">
        <button
          (click)="changeTab('overview')"
          class="py-2 px-1 border-b-2 text-sm font-medium"
          [class]="activeTab === 'overview'
            ? 'border-indigo-500 text-indigo-600'
            : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
        >
          Overview
        </button>
        <button
          (click)="changeTab('tasks')"
          class="py-2 px-1 border-b-2 text-sm font-medium"
          [class]="activeTab === 'tasks'
            ? 'border-indigo-500 text-indigo-600'
            : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
        >
          Task Analytics
        </button>
        <button
          *ngIf="isAdminOrManager"
          (click)="changeTab('team')"
          class="py-2 px-1 border-b-2 text-sm font-medium"
          [class]="activeTab === 'team'
            ? 'border-indigo-500 text-indigo-600'
            : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
        >
          Team Performance
        </button>
        <button
          (click)="changeTab('metrics')"
          class="py-2 px-1 border-b-2 text-sm font-medium"
          [class]="activeTab === 'metrics'
            ? 'border-indigo-500 text-indigo-600'
            : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
        >
          Metrics
        </button>
      </nav>
    </div>

    <!-- Tab Content -->
    <div class="mt-6">
      <!-- Overview Tab -->
      <div *ngIf="activeTab === 'overview'" class="space-y-6">
        <!-- Project Progress -->
        <div class="bg-white rounded-lg border border-gray-200 p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Project Progress</h3>
          <div class="space-y-4">
            <div *ngFor="let project of projectProgress" class="flex items-center">
              <div class="w-1/4">
                <a [routerLink]="['/projects', project.projectId]" class="text-sm font-medium text-gray-900 hover:text-indigo-600">
                  {{ project.projectName }}
                </a>
                <p class="text-xs text-gray-500">{{ project.totalTasks }} tasks</p>
              </div>
              <div class="w-3/4">
                <div class="flex items-center">
                  <div class="flex-1 h-2 bg-gray-200 rounded-full overflow-hidden">
                    <div
                      class="h-full rounded-full transition-all duration-500"
                      [class]="getProgressColor(project.progress)"
                      [style.width]="project.progress + '%'"
                    ></div>
                  </div>
                  <span class="ml-3 text-sm font-medium text-gray-900 min-w-[50px]">{{ project.progress }}%</span>
                </div>
                <div *ngIf="project.isOverdue" class="text-xs text-red-600 mt-1">
                  ⚠️ Overdue
                </div>
              </div>
            </div>
            <div *ngIf="projectProgress.length === 0" class="text-center py-4 text-gray-500">
              No projects available
            </div>
          </div>
        </div>

        <!-- Recent Activity -->
        <div class="bg-white rounded-lg border border-gray-200 p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Recent Activity</h3>
          <div class="flow-root">
            <ul class="-mb-8">
              <li *ngFor="let activity of recentActivities; let last = last">
                <div class="relative pb-8" [class.last:pb-0]="last">
                  <div *ngIf="!last" class="absolute top-4 left-4 -ml-px h-full w-0.5 bg-gray-200" aria-hidden="true"></div>
                  <div class="relative flex space-x-3">
                    <div>
                      <span class="h-8 w-8 rounded-full bg-indigo-100 flex items-center justify-center ring-8 ring-white">
                        <span class="text-indigo-600 text-xs font-medium">
                          {{ activity.user.charAt(0).toUpperCase() }}
                        </span>
                      </span>
                    </div>
                    <div class="flex-1 min-w-0">
                      <div>
                        <p class="text-sm text-gray-900">
                          <span class="font-medium">{{ activity.user }}</span>
                          {{ activity.action }}
                          <span class="font-medium">{{ activity.entityName }}</span>
                        </p>
                        <p class="text-xs text-gray-500">
                          {{ activity.timestamp | date:'MMM d, y h:mm a' }}
                          <span *ngIf="activity.details" class="ml-2 text-gray-400">• {{ activity.details }}</span>
                        </p>
                      </div>
                    </div>
                    <div class="text-right">
                      <span class="text-xs text-gray-400">{{ activity.entity }}</span>
                    </div>
                  </div>
                </div>
              </li>
              <li *ngIf="recentActivities.length === 0" class="text-center py-4 text-gray-500">
                No recent activity
              </li>
            </ul>
          </div>
        </div>
      </div>

      <!-- Task Analytics Tab -->
      <div *ngIf="activeTab === 'tasks'" class="space-y-6">
        <div *ngIf="loadingTasks" class="flex justify-center items-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
        </div>

        <div *ngIf="!loadingTasks && taskStatistics" class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Tasks by Status -->
          <div class="bg-white rounded-lg border border-gray-200 p-6">
            <h4 class="text-sm font-medium text-gray-700 mb-4">Tasks by Status</h4>
            <div class="space-y-3">
              <div *ngFor="let status of getKeys(taskStatistics.tasksByStatus)" class="flex items-center">
                <span class="w-24 text-sm text-gray-600">{{ getStatusLabel(+status) }}</span>
                <div class="flex-1">
                  <div class="h-2 bg-gray-200 rounded-full overflow-hidden">
                    <div
                      class="h-full rounded-full"
                      [style.width]="(taskStatistics.tasksByStatus[status] / taskStatistics.totalTasks * 100) + '%'"
                      [style.background-color]="getStatusColor(+status)"
                    ></div>
                  </div>
                </div>
                <span class="ml-3 text-sm font-medium text-gray-900">{{ taskStatistics.tasksByStatus[status] }}</span>
              </div>
            </div>
          </div>

          <!-- Tasks by Priority -->
          <div class="bg-white rounded-lg border border-gray-200 p-6">
            <h4 class="text-sm font-medium text-gray-700 mb-4">Tasks by Priority</h4>
            <div class="space-y-3">
              <div *ngFor="let priority of getKeys(taskStatistics.tasksByPriority)" class="flex items-center">
                <span class="w-24 text-sm text-gray-600">{{ getPriorityLabel(+priority) }}</span>
                <div class="flex-1">
                  <div class="h-2 bg-gray-200 rounded-full overflow-hidden">
                    <div
                      class="h-full rounded-full"
                      [style.width]="(taskStatistics.tasksByPriority[priority] / taskStatistics.totalTasks * 100) + '%'"
                      [style.background-color]="getPriorityColor(+priority)"
                    ></div>
                  </div>
                </div>
                <span class="ml-3 text-sm font-medium text-gray-900">{{ taskStatistics.tasksByPriority[priority] }}</span>
              </div>
            </div>
          </div>

          <!-- Task Stats -->
          <div class="bg-white rounded-lg border border-gray-200 p-6 lg:col-span-2">
            <h4 class="text-sm font-medium text-gray-700 mb-4">Task Statistics</h4>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div>
                <p class="text-xs text-gray-500">Total</p>
                <p class="text-xl font-bold text-gray-900">{{ taskStatistics.totalTasks }}</p>
              </div>
              <div>
                <p class="text-xs text-gray-500">Completion Rate</p>
                <p class="text-xl font-bold text-green-600">{{ taskStatistics.completionRate }}%</p>
              </div>
              <div>
                <p class="text-xs text-gray-500">Overdue</p>
                <p class="text-xl font-bold text-red-600">{{ taskStatistics.overdueTasks }}</p>
              </div>
              <div>
                <p class="text-xs text-gray-500">Avg. Completion Time</p>
                <p class="text-xl font-bold text-gray-900">{{ taskStatistics.averageCompletionTime }} days</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Team Performance Tab -->
      <div *ngIf="activeTab === 'team' && isAdminOrManager" class="space-y-6">
        <div class="bg-white rounded-lg border border-gray-200 p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Top Performers</h3>
          <div *ngIf="topPerformers.length > 0" class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead>
                <tr>
                  <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Team Member</th>
                  <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Assigned</th>
                  <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Completed</th>
                  <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Overdue</th>
                  <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Completion Rate</th>
                  <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Productivity Score</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-200">
                <tr *ngFor="let performer of topPerformers">
                  <td class="px-4 py-3">
                    <div class="flex items-center">
                      <div class="w-8 h-8 rounded-full bg-indigo-100 flex items-center justify-center">
                        <span class="text-indigo-600 font-medium text-xs">{{ performer.fullName.charAt(0) }}</span>
                      </div>
                      <div class="ml-3">
                        <p class="text-sm font-medium text-gray-900">{{ performer.fullName }}</p>
                        <p class="text-xs text-gray-500">@{{ performer.userName }}</p>
                      </div>
                    </div>
                  </td>
                  <td class="px-4 py-3 text-center text-sm text-gray-900">{{ performer.assignedTasks }}</td>
                  <td class="px-4 py-3 text-center text-sm text-green-600">{{ performer.completedTasks }}</td>
                  <td class="px-4 py-3 text-center text-sm text-red-600">{{ performer.overdueTasks }}</td>
                  <td class="px-4 py-3 text-center text-sm">
                    <span class="inline-block px-2 py-1 text-xs font-semibold rounded-full" [class]="performer.completionRate >= 70 ? 'bg-green-100 text-green-800' : performer.completionRate >= 40 ? 'bg-yellow-100 text-yellow-800' : 'bg-red-100 text-red-800'">
                      {{ performer.completionRate }}%
                    </span>
                  </td>
                  <td class="px-4 py-3 text-center text-sm font-bold text-indigo-600">{{ performer.productivityScore }}</td>
                </tr>
              </tbody>
            </table>
          </div>
          <div *ngIf="topPerformers.length === 0" class="text-center py-4 text-gray-500">
            No performance data available
          </div>
        </div>
      </div>

      <!-- Metrics Tab -->
      <div *ngIf="activeTab === 'metrics'" class="space-y-6">
        <div *ngIf="performanceMetrics" class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Key Metrics -->
          <div class="bg-white rounded-lg border border-gray-200 p-6 md:col-span-2">
            <h4 class="text-sm font-medium text-gray-700 mb-4">Key Metrics</h4>
            <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
              <div class="text-center p-4 bg-gray-50 rounded-lg">
                <p class="text-xs text-gray-500">Productivity</p>
                <p class="text-2xl font-bold text-indigo-600">{{ performanceMetrics.overallProductivity }}%</p>
              </div>
              <div class="text-center p-4 bg-gray-50 rounded-lg">
                <p class="text-xs text-gray-500">Project Success Rate</p>
                <p class="text-2xl font-bold text-green-600">{{ performanceMetrics.projectSuccessRate }}%</p>
              </div>
              <div class="text-center p-4 bg-gray-50 rounded-lg">
                <p class="text-xs text-gray-500">Task Efficiency</p>
                <p class="text-2xl font-bold text-blue-600">{{ performanceMetrics.taskEfficiency }}%</p>
              </div>
              <div class="text-center p-4 bg-gray-50 rounded-lg">
                <p class="text-xs text-gray-500">On-Time Delivery</p>
                <p class="text-2xl font-bold text-green-600">{{ performanceMetrics.onTimeDeliveryRate }}%</p>
              </div>
              <div class="text-center p-4 bg-gray-50 rounded-lg">
                <p class="text-xs text-gray-500">Resource Utilization</p>
                <p class="text-2xl font-bold text-purple-600">{{ performanceMetrics.resourceUtilization }}%</p>
              </div>
            </div>
          </div>

          <!-- Metrics by Project -->
          <div class="bg-white rounded-lg border border-gray-200 p-6">
            <h4 class="text-sm font-medium text-gray-700 mb-4">Metrics by Project</h4>
            <div class="space-y-3">
              <div *ngFor="let item of performanceMetrics.metricsByProject | keyvalue" class="flex items-center">
                <span class="w-1/3 text-sm text-gray-600 truncate">{{ item.key }}</span>
                <div class="flex-1">
                  <div class="h-2 bg-gray-200 rounded-full overflow-hidden">
                    <div
                      class="h-full rounded-full bg-indigo-500"
                      [style.width]="item.value + '%'"
                    ></div>
                  </div>
                </div>
                <span class="ml-3 text-sm font-medium text-gray-900">{{ item.value }}%</span>
              </div>
            </div>
          </div>

          <!-- Metric Trend -->
          <div class="bg-white rounded-lg border border-gray-200 p-6">
            <h4 class="text-sm font-medium text-gray-700 mb-4">Productivity Trend</h4>
            <div class="space-y-3">
              <div *ngFor="let trend of performanceMetrics.metricsTrend.slice(-10)" class="flex items-center">
                <span class="w-16 text-xs text-gray-500">{{ trend.key }}</span>
                <div class="flex-1">
                  <div class="h-2 bg-gray-200 rounded-full overflow-hidden">
                    <div
                      class="h-full rounded-full"
                      [class]="trend.value >= 70 ? 'bg-green-500' : trend.value >= 40 ? 'bg-yellow-500' : 'bg-red-500'"
                      [style.width]="trend.value + '%'"
                    ></div>
                  </div>
                </div>
                <span class="ml-3 text-sm font-medium text-gray-900">{{ trend.value }}%</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
```

## Dashboard Service
```ts
// src/app/core/services/dashboard.service.ts
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { DashboardSummary, ProjectProgress, UserPerformance, PerformanceMetrics, RecentActivity } from '../models/dashboard.model';

@Injectable({
    providedIn: 'root'
})
export class DashboardService {
    constructor(private apiService: ApiService) { }

    getDashboardSummary(filter: { daysToShow: number; projectId?: string }): Observable<DashboardSummary> {
        return this.apiService.get<DashboardSummary>('dashboard/summary', filter);
    }

    getProjectProgress(projectId: string): Observable<ProjectProgress> {
        return this.apiService.get<ProjectProgress>(`dashboard/projects/${projectId}/progress`);
    }

    getAllProjectsProgress(): Observable<ProjectProgress[]> {
        return this.apiService.get<ProjectProgress[]>('dashboard/projects/progress');
    }

    getUserPerformance(userId: string): Observable<UserPerformance> {
        return this.apiService.get<UserPerformance>(`dashboard/users/${userId}/performance`);
    }

    getTeamPerformance(): Observable<UserPerformance[]> {
        return this.apiService.get<UserPerformance[]>('dashboard/team/performance');
    }

    getPerformanceMetrics(filter: { daysToShow: number; projectId?: string }): Observable<PerformanceMetrics> {
        return this.apiService.get<PerformanceMetrics>('dashboard/metrics', filter);
    }

    getRecentActivities(count: number = 10, userId?: string): Observable<RecentActivity[]> {
        const params: any = { count };
        if (userId) params.userId = userId;
        return this.apiService.get<RecentActivity[]>('dashboard/activities', params);
    }
}
```

# Part 14 : Solved Bugs to Run Frontend

```bash

# click the commit to check what was changed
```