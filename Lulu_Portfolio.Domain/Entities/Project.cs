namespace Lulu_Portfolio.Domain.Entities;

public class Project : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public string LiveUrl { get; set; } = string.Empty;

    public string GithubUrl { get; set; } = string.Empty;

    public bool IsFeatured { get; set; }
}