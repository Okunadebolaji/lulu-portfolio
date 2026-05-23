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

        // ✅ PUBLIC: Get all projects
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _context.Projects
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(ApiResponse<List<Project>>.SuccessResponse(
                projects,
                "Projects retrieved successfully"
            ));
        }

        // ✅ PUBLIC: Get single project
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound(ApiResponse<object>.FailResponse(
                    "Project not found"
                ));
            }

            return Ok(ApiResponse<Project>.SuccessResponse(
                project,
                "Project retrieved successfully"
            ));
        }

        // 🔐 ADMIN ONLY: Create project
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectDto dto)
        {
            try
            {
                // ✅ Validate thumbnail URL is provided
                if (string.IsNullOrEmpty(dto.ThumbnailUrl))
                {
                    return BadRequest(ApiResponse<object>.FailResponse(
                        "Thumbnail URL is required. Upload image first."
                    ));
                }

                var project = new Project
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ThumbnailUrl = dto.ThumbnailUrl, // ✅ Use uploaded URL
                    LiveUrl = dto.LiveUrl,
                    GithubUrl = dto.GithubUrl,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<Project>.SuccessResponse(
                    project,
                    "Project created successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "Failed to create project",
                    new List<string> { ex.Message }
                ));
            }
        }

        // 🔐 ADMIN ONLY: Update project
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound(ApiResponse<object>.FailResponse(
                    "Project not found"
                ));
            }

            project.Title = dto.Title;
            project.Description = dto.Description;
            
            // ✅ Only update thumbnail if new URL provided
            if (!string.IsNullOrEmpty(dto.ThumbnailUrl))
            {
                project.ThumbnailUrl = dto.ThumbnailUrl;
            }
            
            project.LiveUrl = dto.LiveUrl;
            project.GithubUrl = dto.GithubUrl;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Project>.SuccessResponse(
                project,
                "Project updated successfully"
            ));
        }

        // 🔐 ADMIN ONLY: Delete project
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound(ApiResponse<object>.FailResponse(
                    "Project not found"
                ));
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                null!,
                "Project deleted successfully"
            ));
        }

        // 🔐 ADMIN ONLY: Upload project image
        // Returns: { success: true, url: "http://localhost:5113/uploads/abc.png" }
        [Authorize(Roles = "Admin")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // ✅ Validation
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "No file uploaded"
                ));
            }

            // ✅ File type validation
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    $"Invalid file type. Allowed: {string.Join(", ", allowedExtensions)}"
                ));
            }

            // ✅ File size validation (max 5MB)
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "File size exceeds 5MB limit"
                ));
            }

            try
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads"
                );

                // ✅ Create uploads folder if missing
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // ✅ Generate unique filename (GUID + extension)
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, fileName);

                // ✅ Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // ✅ Generate and return URL
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

                return Ok(new
                {
                    success = true,
                    url = fileUrl,
                    fileName = fileName,
                    message = "Image uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "Failed to upload image",
                    new List<string> { ex.Message }
                ));
            }
        }
    }
}