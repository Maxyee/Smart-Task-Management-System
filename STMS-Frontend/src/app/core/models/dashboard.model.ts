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