using SmartTaskManagement.Application.DTOs.Shared;
using SmartTaskManagement.Domain.Enums;

namespace SmartTaskManagement.Application.DTOs.Tasks
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public TaskItemPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public Guid? AssignedToUserId { get; set; }
        public UserDto? AssignedToUser { get; set; }
        public UserDto? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysUntilDue { get; set; }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
        public DateTime DueDate { get; set; }
        public int EstimatedHours { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedToUserId { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public Guid? AssignedToUserId { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        public TaskItemStatus Status { get; set; }
        public string? Comment { get; set; }
    }

    public class AssignTaskDto
    {
        public Guid UserId { get; set; }
    }

    public class TaskFilterDto
    {
        public string? SearchTerm { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public TaskItemStatus? Status { get; set; }
        public TaskItemPriority? Priority { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public bool? IsOverdue { get; set; }
        public bool? ShowOnlyAssignedToMe { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "DueDate";
        public bool SortDescending { get; set; } = false;
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
        public Dictionary<TaskItemPriority, int> TasksByPriority { get; set; } = new();
        public Dictionary<TaskItemStatus, int> TasksByStatus { get; set; } = new();
        public double CompletionRate { get; set; }
    }

    public class TaskActivityDto
    {
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public string? Details { get; set; }
    }
}