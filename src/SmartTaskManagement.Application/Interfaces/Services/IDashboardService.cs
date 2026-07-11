using SmartTaskManagement.Application.Common;
using SmartTaskManagement.Application.DTOs.Dashboard;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<Response<DashboardSummaryDto>> GetDashboardSummaryAsync(DashboardFilterDto filter, Guid userId);
        Task<Response<ProjectProgressDto>> GetProjectProgressAsync(Guid projectId, Guid userId);
        Task<Response<IEnumerable<ProjectProgressDto>>> GetAllProjectsProgressAsync(Guid userId);
        Task<Response<UserPerformanceDto>> GetUserPerformanceAsync(Guid userId, Guid requestingUserId);
        Task<Response<IEnumerable<UserPerformanceDto>>> GetTeamPerformanceAsync(Guid requestingUserId);
        Task<Response<PerformanceMetricsDto>> GetPerformanceMetricsAsync(DashboardFilterDto filter, Guid userId);
        Task<Response<IEnumerable<RecentActivityDto>>> GetRecentActivitiesAsync(int count = 10, Guid? userId = null);
    }
}