// ============================================
// File 1: DashboardStatsDto.cs
// Location: Lulu_Portfolio.API/Models/DTOs/Dashboard/
// ============================================

namespace Lulu_Portfolio.API.Models.DTOs.Dashboard
{
    /// <summary>
    /// Dashboard statistics DTO
    /// Maps to Result Set 1 from sp_GetDashboardStats stored procedure
    /// </summary>
    public class DashboardStatsDto
    {
        /// <summary>Total number of projects in portfolio</summary>
        public int TotalProjects { get; set; }

        /// <summary>Total number of skills</summary>
        public int TotalSkills { get; set; }

        /// <summary>Average proficiency percentage across all skills</summary>
        public decimal AvgProficiency { get; set; }

        /// <summary>Number of unique skill categories</summary>
        public int TotalCategories { get; set; }

        /// <summary>Number of featured projects</summary>
        public int FeaturedProjects { get; set; }
    }
}

// ============================================
// File 2: RecentProjectDto.cs
// Location: Lulu_Portfolio.API/Models/DTOs/Dashboard/
// ============================================

namespace Lulu_Portfolio.API.Models.DTOs.Dashboard
{
    /// <summary>
    /// Recent project DTO
    /// Maps to Result Set 2 from sp_GetDashboardStats stored procedure
    /// </summary>
    public class RecentProjectDto
    {
        /// <summary>Project ID</summary>
        public int Id { get; set; }

        /// <summary>Project title</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Project description</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>URL to project thumbnail image</summary>
        public string ThumbnailUrl { get; set; } = string.Empty;

        /// <summary>URL to live project</summary>
        public string LiveUrl { get; set; } = string.Empty;

        /// <summary>URL to project GitHub repository</summary>
        public string GithubUrl { get; set; } = string.Empty;

        /// <summary>Whether this project is featured</summary>
        public bool IsFeatured { get; set; }

        /// <summary>Project creation date</summary>
        public DateTime CreatedAt { get; set; }
    }
}

// ============================================
// File 3: DashboardResponseDto.cs
// Location: Lulu_Portfolio.API/Models/DTOs/Dashboard/
// ============================================

namespace Lulu_Portfolio.API.Models.DTOs.Dashboard
{
    /// <summary>
    /// Dashboard response DTO
    /// Combines both result sets from sp_GetDashboardStats
    /// </summary>
    public class DashboardResponseDto
    {
        /// <summary>Dashboard statistics</summary>
        public DashboardStatsDto Stats { get; set; } = new();

        /// <summary>Recent projects (up to 5)</summary>
        public List<RecentProjectDto> RecentProjects { get; set; } = new();
    }
}