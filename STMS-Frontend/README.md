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