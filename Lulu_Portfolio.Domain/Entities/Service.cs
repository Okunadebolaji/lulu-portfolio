namespace Lulu_Portfolio.Domain.Entities;

public class Service : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
}