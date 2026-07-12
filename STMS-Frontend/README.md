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
