using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lulu_Portfolio.Infrastructure.Persistence;
using Lulu_Portfolio.Domain.Entities;
using Lulu_Portfolio.API.Models.DTOs.Project;
using Lulu_Portfolio.API.Models;

namespace Lulu_Portfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var projects = await _context.Projects
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Projects retrieved successfully",
                    data = projects
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to retrieve projects",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Project not found"));
                }

                return Ok(new
                {
                    success = true,
                    message = "Project retrieved successfully",
                    data = project
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to retrieve project",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Title))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("Title is required"));
                }

                var project = new Project
                {
                    Title = dto.Title.Trim(),
                    Description = dto.Description?.Trim() ?? string.Empty,
                    ThumbnailUrl = dto.ThumbnailUrl?.Trim() ?? string.Empty,
                    LiveUrl = dto.LiveUrl?.Trim() ?? string.Empty,
                    GithubUrl = dto.GithubUrl?.Trim() ?? string.Empty,
                    IsFeatured = dto.IsFeatured,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Project created successfully",
                    data = project
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to create project",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Project not found"));
                }

                project.Title = dto.Title.Trim();
                project.Description = dto.Description?.Trim() ?? string.Empty;
                
                if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl))
                {
                    project.ThumbnailUrl = dto.ThumbnailUrl.Trim();
                }
                
                project.LiveUrl = dto.LiveUrl?.Trim() ?? string.Empty;
                project.GithubUrl = dto.GithubUrl?.Trim() ?? string.Empty;
                project.IsFeatured = dto.IsFeatured;
                project.UpdatedAt = DateTime.Now.ToUniversalTime();

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Project updated successfully",
                    data = project
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to update project",
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
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Project not found"));
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Project deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to delete project",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No file uploaded"
                    });
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Invalid file type. Allowed: {string.Join(", ", allowedExtensions)}"
                    });
                }

                const long maxFileSize = 5 * 1024 * 1024;
                if (file.Length > maxFileSize)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "File size exceeds 5MB limit"
                    });
                }

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

                return Ok(new
                {
                    success = true,
                    message = "Image uploaded successfully",
                    url = fileUrl,
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to upload image",
                    error = ex.Message
                });
            }
        }
    }
}