using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lulu_Portfolio.Infrastructure.Persistence;
using Lulu_Portfolio.Domain.Entities;
using Lulu_Portfolio.API.Models.DTOs.Project;
using Lulu_Portfolio.API.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Lulu_Portfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public ProjectController(AppDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
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
                    return NotFound(new
                    {
                        success = false,
                        message = "Project not found"
                    });
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var project = new Project
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ThumbnailUrl = dto.ThumbnailUrl,
                    LiveUrl = dto.LiveUrl,
                    GithubUrl = dto.GithubUrl,
                    IsFeatured = dto.IsFeatured
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = project.Id }, new
                {
                    success = true,
                    message = "Project created successfully",
                    data = project
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error occurred",
                    error = ex.InnerException?.Message ?? ex.Message
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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Project not found"
                    });
                }

                project.Title = dto.Title;
                project.Description = dto.Description;
                project.ThumbnailUrl = dto.ThumbnailUrl;
                project.LiveUrl = dto.LiveUrl;
                project.GithubUrl = dto.GithubUrl;
                project.IsFeatured = dto.IsFeatured;
                project.UpdatedAt = DateTime.UtcNow;

                _context.Projects.Update(project);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Project updated successfully",
                    data = project
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error occurred",
                    error = ex.InnerException?.Message ?? ex.Message
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

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var project = await _context.Projects.FindAsync(id);

                if (project == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Project not found"
                    });
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Project deleted successfully",
                    data = project
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

        [HttpPost("upload")]
        [Authorize]
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

                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "lulu-portfolio/projects",
                        Overwrite = false
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        return StatusCode(500, new
                        {
                            success = false,
                            message = "Failed to upload image to cloud",
                            error = uploadResult.Error.Message
                        });
                    }

                    return Ok(new
                    {
                        success = true,
                        message = "Image uploaded successfully",
                        url = uploadResult.SecureUrl.ToString(),
                        publicId = uploadResult.PublicId
                    });
                }
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