using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lulu_Portfolio.Infrastructure.Persistence;
using Lulu_Portfolio.Domain.Entities;
using Lulu_Portfolio.API.Models.DTOs;
using Lulu_Portfolio.API.Models;

namespace Lulu_Portfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestimonialsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestimonialsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var testimonials = await _context.Testimonials
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Testimonials retrieved successfully",
                    data = testimonials
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to retrieve testimonials",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var testimonial = await _context.Testimonials.FindAsync(id);

                if (testimonial == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Testimonial not found"));
                }

                return Ok(new
                {
                    success = true,
                    message = "Testimonial retrieved successfully",
                    data = testimonial
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to retrieve testimonial",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTestimonialDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.ClientName))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("Client name is required"));
                }

                if (string.IsNullOrWhiteSpace(dto.Comment))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("Comment is required"));
                }

                var testimonial = new Testimonial
                {
                    ClientName = dto.ClientName.Trim(),
                    Company = dto.Company?.Trim() ?? string.Empty,
                    Comment = dto.Comment.Trim(),
                    Rating = dto.Rating,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Testimonials.Add(testimonial);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Testimonial created successfully",
                    data = testimonial
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to create testimonial",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTestimonialDto dto)
        {
            try
            {
                var testimonial = await _context.Testimonials.FindAsync(id);

                if (testimonial == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Testimonial not found"));
                }

                testimonial.ClientName = dto.ClientName.Trim();
                testimonial.Company = dto.Company?.Trim() ?? string.Empty;
                testimonial.Comment = dto.Comment.Trim();
                testimonial.Rating = dto.Rating;
                testimonial.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Testimonial updated successfully",
                    data = testimonial
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to update testimonial",
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
                var testimonial = await _context.Testimonials.FindAsync(id);

                if (testimonial == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Testimonial not found"));
                }

                _context.Testimonials.Remove(testimonial);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Testimonial deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to delete testimonial",
                    new List<string> { ex.Message }
                ));
            }
        }
    }
}