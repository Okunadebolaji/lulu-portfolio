using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lulu_Portfolio.API.Services.Interfaces;
using Lulu_Portfolio.API.Services;
using Lulu_Portfolio.API.Models;
using Lulu_Portfolio.Infrastructure.Persistence;

namespace Lulu_Portfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;
        private readonly AppDbContext _context;

        public DashboardController(
            IDashboardService dashboardService,
            ILogger<DashboardController> logger,
            AppDbContext context)
        {
            _dashboardService = dashboardService;
            _logger = logger;
            _context = context;
        }

        // GET: api/dashboard/stats
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                _logger.LogInformation("Dashboard stats requested");

                var stats = await _dashboardService.GetDashboardStatsAsync();

                return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(
                    stats,
                    "Dashboard statistics retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard stats");

                return StatusCode(500,
                    ApiResponse<object>.FailResponse(
                        "Failed to retrieve dashboard statistics",
                        new List<string> { ex.Message }
                    )
                );
            }
        }

        // GET: api/dashboard/quick-stats
        [HttpGet("quick-stats")]
        [ProducesResponseType(typeof(ApiResponse<QuickStatsDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetQuickStats()
        {
            try
            {
                var totalProjects = await _context.Projects
                    .AsNoTracking()
                    .CountAsync();

                var totalSkills = await _context.Skills
                    .AsNoTracking()
                    .CountAsync();

                var quickStats = new QuickStatsDto
                {
                    TotalProjects = totalProjects,
                    TotalSkills = totalSkills
                };

                return Ok(ApiResponse<QuickStatsDto>.SuccessResponse(
                    quickStats,
                    "Quick stats retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quick stats");

                return StatusCode(500,
                    ApiResponse<object>.FailResponse(
                        "Failed to retrieve quick stats",
                        new List<string> { ex.Message }
                    )
                );
            }
        }
    }

    public class QuickStatsDto
    {
        public int TotalProjects { get; set; }
        public int TotalSkills { get; set; }
    }
}