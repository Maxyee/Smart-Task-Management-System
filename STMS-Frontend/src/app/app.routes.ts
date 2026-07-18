import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';
//import { ProjectFormComponent } from './features/projects/project-form/project-form.component';
//import TaskFormComponent from './features/tasks/task-form/task-form.component';

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
    path: 'chat',
    loadComponent: () => import('./features/chat/chat.component').then(m => m.ChatComponent),
    canActivate: [AuthGuard]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
