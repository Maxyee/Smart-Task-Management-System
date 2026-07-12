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