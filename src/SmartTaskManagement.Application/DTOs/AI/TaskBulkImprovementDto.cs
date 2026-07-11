namespace SmartTaskManagement.Application.DTOs.AI
{
    public class TaskBulkImprovementDto
    {
        public List<Guid> TaskIds { get; set; } = new();
        public ImprovementOptions Options { get; set; } = new();
    }

    public class TaskBulkImprovementResponseDto
    {
        public int TotalProcessed { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public List<TaskImprovementResultDto> Results { get; set; } = new();
    }

    public class TaskImprovementResultDto
    {
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public TaskImprovementResponseDto? Improvement { get; set; }
    }
}