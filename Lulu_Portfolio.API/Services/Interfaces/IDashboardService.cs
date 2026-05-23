using Lulu_Portfolio.API.Models.DTOs;

namespace Lulu_Portfolio.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        
    }
}