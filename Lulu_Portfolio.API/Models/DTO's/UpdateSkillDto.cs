namespace Lulu_Portfolio.API.Models.DTOs.Skill
{
    public class UpdateSkillDto
    {
        public string Name { get; set; } = string.Empty;

        public int Percentage { get; set; }

        public string Category { get; set; } = string.Empty;
    }
}