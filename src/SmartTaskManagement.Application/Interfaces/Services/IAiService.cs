using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.AI;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IAiService
    {
        /// <summary>
        /// Improve a single task description using AI
        /// </summary>
        Task<Response<TaskImprovementResponseDto>> ImproveTaskDescriptionAsync(
            TaskImprovementRequestDto request, Guid userId);

        /// <summary>
        /// Improve multiple task descriptions in bulk
        /// </summary>
        Task<Response<TaskBulkImprovementResponseDto>> BulkImproveTaskDescriptionsAsync(
            TaskBulkImprovementDto bulkRequest, Guid userId);

        /// <summary>
        /// Generate a summary for a task
        /// </summary>
        Task<Response<string>> GenerateTaskSummaryAsync(string description, Guid userId);

        /// <summary>
        /// Suggest next actions for a task
        /// </summary>
        Task<Response<List<string>>> SuggestNextActionsAsync(
            string title, string description, string status, Guid userId);

        /// <summary>
        /// Check if AI service is available
        /// </summary>
        Task<Response<bool>> CheckHealthAsync();
    }
}