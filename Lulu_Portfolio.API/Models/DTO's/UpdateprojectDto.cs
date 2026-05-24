namespace Lulu_Portfolio.API.Models.DTOs.Project
{
    public class UpdateProjectDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string LiveUrl { get; set; } = string.Empty;
        public string GithubUrl { get; set; } = string.Empty;
        public bool IsFeatured { get; set; } = false;
    }
}