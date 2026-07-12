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