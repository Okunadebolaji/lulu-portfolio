using Lulu_Portfolio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lulu_Portfolio.API.Services.Interfaces;
using Lulu_Portfolio.API.Models.DTOs;

namespace Lulu_Portfolio.API.Services
{
/// <summary>
/// Service for dashboard statistics with optimized queries
/// CORRECTED: Uses actual database fields (Percentage, not Proficiency)
/// </summary>

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves comprehensive dashboard statistics
    /// Uses actual Skill.Percentage field (not Proficiency)
    /// No per-user filtering (all data)
    /// </summary>
    public async Task<DashboardStatsDto> GetDashboardStats()
    {
        try
        {
            // ✅ Count total projects
            var totalProjects = await _context.Projects
                .AsNoTracking()
                .CountAsync();

            // ✅ Count total skills
            var totalSkills = await _context.Skills
                .AsNoTracking()
                .CountAsync();

            // ✅ Calculate average proficiency from Percentage field
            var avgProficiency = 0;
            var skillsWithPercentage = await _context.Skills
                .AsNoTracking()
                .Where(s => s.Percentage>0)
                .AverageAsync(s => (double?)s.Percentage);
            
            if (skillsWithPercentage.HasValue)
            {
                avgProficiency = (int)Math.Round(skillsWithPercentage.Value);
            }

            // ✅ Count unique categories
            var totalCategories = await _context.Skills
                .AsNoTracking()
                .Select(s => s.Category)
                .Distinct()
                .CountAsync();

            // ✅ Count featured projects
            var featuredProjects = await _context.Projects
                .AsNoTracking()
                .CountAsync(p => p.IsFeatured);

            // ✅ Get recent projects (5 most recent)
            var recentProjects = await _context.Projects
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new ProjectSummaryDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    ThumbnailUrl = p.ThumbnailUrl,
                    LiveUrl = p.LiveUrl,
                    GithubUrl = p.GithubUrl,
                    IsFeatured = p.IsFeatured,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return new DashboardStatsDto
            {
                TotalProjects = totalProjects,
                TotalSkills = totalSkills,
                AverageProficiency = avgProficiency,
                TotalCategories = totalCategories,
                FeaturedProjects = featuredProjects,
                RecentProjects = recentProjects
            };
        }
        catch (Exception ex)
        {
            // Log exception here
            Console.WriteLine($"Error in GetDashboardStats: {ex.Message}");
            throw;
        }
    }

        public Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            throw new NotImplementedException();
        }
    }

/// <summary>
/// DTO for Dashboard Statistics
/// ✅ CORRECTED: Uses Percentage (not Proficiency)
/// ✅ CORRECTED: Includes FeaturedProjects
/// </summary>
public class DashboardStatsDto
{
    public int TotalProjects { get; set; }
    public int TotalSkills { get; set; }
    public int AverageProficiency { get; set; } // 0-100 from Percentage field
    public int TotalCategories { get; set; }
    public int FeaturedProjects { get; set; }
    public List<ProjectSummaryDto> RecentProjects { get; set; } = new();
}

/// <summary>
/// DTO for Project Summary (Recent Projects)
/// ✅ CORRECTED: Matches actual Project model fields
/// </summary>
public class ProjectSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string LiveUrl { get; set; } = string.Empty;
    public string GithubUrl { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
}

}