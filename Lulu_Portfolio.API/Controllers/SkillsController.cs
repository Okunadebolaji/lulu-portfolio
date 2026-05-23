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
            var skills = await _context.Skills
                .OrderBy(x => x.Category)
                .ToListAsync();

            return Ok(ApiResponse<List<Skill>>.SuccessResponse(
                skills,
                "Skills retrieved successfully"
            ));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound(ApiResponse<object>.FailResponse(
                    "Skill not found"
                ));
            }

            return Ok(ApiResponse<Skill>.SuccessResponse(
                skill,
                "Skill retrieved successfully"
            ));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSkillDto dto)
        {
            try
            {
                var skill = new Skill
                {
                    Name = dto.Name,
                    Percentage = dto.Percentage,
                    Category = dto.Category,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<Skill>.SuccessResponse(
                    skill,
                    "Skill created successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "Failed to create skill",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateSkillDto dto)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound(ApiResponse<object>.FailResponse(
                    "Skill not found"
                ));
            }

            skill.Name = dto.Name;
            skill.Percentage = dto.Percentage;
            skill.Category = dto.Category;
            skill.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Skill>.SuccessResponse(
                skill,
                "Skill updated successfully"
            ));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound(ApiResponse<object>.FailResponse(
                    "Skill not found"
                ));
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                "Skill deleted successfully"
            ));
        }
    }
}