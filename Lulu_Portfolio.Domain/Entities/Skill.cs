public class Skill
{
public int Id { get; set; }
public string? Name { get; set; }
public int Percentage { get; set; }  // ← This is the correct field name
public string? Category { get; set; }
public DateTime CreatedAt { get; set; }
public DateTime? UpdatedAt { get; set; }
// NO Description field
// NO UserId field
// NO Proficiency field
}