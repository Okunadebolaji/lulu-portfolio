using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lulu_Portfolio.Infrastructure.Persistence;
using Lulu_Portfolio.Domain.Entities;
using Lulu_Portfolio.API.Models.DTOs.Skill;
using Lulu_Portfolio.API.Models;

namespace Lulu_Portfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SkillsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var skills = await _context.Skills
                    .OrderBy(x => x.Category)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Skills retrieved successfully",
                    data = skills
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to retrieve skills",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var skill = await _context.Skills.FindAsync(id);

                if (skill == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Skill not found"));
                }

                return Ok(new
                {
                    success = true,
                    message = "Skill retrieved successfully",
                    data = skill
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to retrieve skill",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSkillDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("Skill name is required"));
                }

                var skill = new Skill
                {
                    Name = dto.Name.Trim(),
                    Percentage = dto.Percentage,
                    Category = dto.Category?.Trim() ?? "Other",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Skill created successfully",
                    data = skill
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to create skill",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSkillDto dto)
        {
            try
            {
                var skill = await _context.Skills.FindAsync(id);

                if (skill == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Skill not found"));
                }

                skill.Name = dto.Name.Trim();
                skill.Percentage = dto.Percentage;
                skill.Category = dto.Category?.Trim() ?? "Other";
                skill.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Skill updated successfully",
                    data = skill
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to update skill",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var skill = await _context.Skills.FindAsync(id);

                if (skill == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Skill not found"));
                }

                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Skill deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to delete skill",
                    new List<string> { ex.Message }
                ));
            }
        }
    }
}