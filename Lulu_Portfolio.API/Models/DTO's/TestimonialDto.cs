namespace Lulu_Portfolio.API.Models.DTOs;

public class CreateTestimonialDto
{
    public string ClientName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
}

public class UpdateTestimonialDto
{
    public string ClientName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
}