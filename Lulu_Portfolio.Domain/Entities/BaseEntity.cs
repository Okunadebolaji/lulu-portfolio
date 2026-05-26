namespace Lulu_Portfolio.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();

    public DateTime? UpdatedAt { get; set; }
}