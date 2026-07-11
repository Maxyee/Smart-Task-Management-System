using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public ProjectStatisticsDto ProjectStatistics { get; set; } = new();
        public TaskStatisticsDto TaskStatistics { get; set; } = new();
        public UserStatisticsDto UserStatistics { get; set; } = new();
        public ActivitySummaryDto RecentActivity { get; set; } = new();
        public PerformanceMetricsDto PerformanceMetrics { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class ProjectStatisticsDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int InactiveProjects { get; set; }
        public int ProjectsCompleted { get; set; }
        public int ProjectsInProgress { get; set; }
        public double ProjectCompletionRate { get; set; }
        public List<ProjectProgressDto> ProjectProgress { get; set; } = new();
        public Dictionary<DateTime, int> ProjectsByMonth { get; set; } = new();
    }

    public class ProjectProgressDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public double Progress { get; set; } // Percentage
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class TaskStatisticsDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int ToDoTasks { get; set; }
        public int CancelledTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int TasksDueThisWeek { get; set; }
        public int TasksDueNextWeek { get; set; }
        public double CompletionRate { get; set; }
        public double AverageCompletionTime { get; set; } // In days
        public Dictionary<TaskItemStatus, int> TasksByStatus { get; set; } = new();
        public Dictionary<TaskItemPriority, int> TasksByPriority { get; set; } = new();
        public Dictionary<string, int> TasksByAssignee { get; set; } = new();
        public List<TaskTrendDto> TaskTrends { get; set; } = new();
    }

    public class TaskTrendDto
    {
        public DateTime Period { get; set; }
        public int Created { get; set; }
        public int Completed { get; set; }
        public int InProgress { get; set; }
    }

    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new();
        public Dictionary<string, int> UsersByActivity { get; set; } = new();
        public List<UserPerformanceDto> TopPerformers { get; set; } = new();
    }

    public class UserPerformanceDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int AssignedTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public double CompletionRate { get; set; }
        public double AverageCompletionTime { get; set; }
        public double ProductivityScore { get; set; }
    }

    public class ActivitySummaryDto
    {
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
        public Dictionary<string, int> ActivityTypes { get; set; } = new();
        public Dictionary<DateTime, int> ActivityTimeline { get; set; } = new();
    }

    public class RecentActivityDto
    {
        public DateTime Timestamp { get; set; }
        public string User { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? Details { get; set; }
    }

    public class PerformanceMetricsDto
    {
        public double OverallProductivity { get; set; }
        public double ProjectSuccessRate { get; set; }
        public double TaskEfficiency { get; set; }
        public double OnTimeDeliveryRate { get; set; }
        public double ResourceUtilization { get; set; }
        public Dictionary<string, double> MetricsByProject { get; set; } = new();
        public List<KeyValuePair<string, double>> MetricsTrend { get; set; } = new();
    }

    public class DashboardFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public int DaysToShow { get; set; } = 30;
    }
}