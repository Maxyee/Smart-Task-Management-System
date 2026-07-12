import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiService } from './api.service';
import { User, UserFilter } from '../models/user.model';
import { PagedResponse } from '../models/project.model';
import { UserPerformance } from '../models/dashboard.model';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    constructor(private apiService: ApiService) { }

    /**
     * Get all users with filtering and pagination
     */
    getUsers(filter: UserFilter): Observable<PagedResponse<User>> {
        return this.apiService.get<PagedResponse<User>>('users', filter);
    }

    /**
     * Get user by ID
     */
    getUserById(id: string): Observable<User> {
        return this.apiService.get<User>(`users/${id}`);
    }

    /**
     * Get all users (simple list, no pagination)
     * Useful for dropdowns and assignments
     */
    getAllUsers(): Observable<User[]> {
        // Get all users with a large page size
        const filter: UserFilter = {
            pageNumber: 1,
            pageSize: 999,
            sortBy: 'username',
            sortDescending: false
        };
        return this.apiService.get<PagedResponse<User>>('users', filter)
            .pipe(
                map(response => response.items)
            );
    }

    /**
     * Get users by role
     */
    getUsersByRole(role: string): Observable<User[]> {
        return this.apiService.get<User[]>(`users/role/${role}`);
    }

    /**
     * Get active users (for assignment)
     */
    getActiveUsers(): Observable<User[]> {
        return this.apiService.get<User[]>('users/active');
    }

    /**
     * Update user
     */
    updateUser(id: string, user: Partial<User>): Observable<User> {
        return this.apiService.put<User>(`users/${id}`, user);
    }

    /**
     * Activate/deactivate user
     */
    toggleUserStatus(id: string): Observable<User> {
        return this.apiService.patch<User>(`users/${id}/toggle-status`, {});
    }

    /**
     * Delete user
     */
    deleteUser(id: string): Observable<void> {
        return this.apiService.delete<void>(`users/${id}`);
    }

    /**
     * Get user performance
     */
    getUserPerformance(id: string): Observable<UserPerformance> {
        return this.apiService.get<UserPerformance>(`users/${id}/performance`);
    }

    /**
     * Get users available for assignment (active + team members + project managers)
     */
    getUsersForAssignment(): Observable<User[]> {
        return this.apiService.get<User[]>('users/available-for-assignment');
    }

    /**
     * Search users by name or email
     */
    searchUsers(searchTerm: string): Observable<User[]> {
        return this.apiService.get<User[]>('users/search', { searchTerm });
    }
}