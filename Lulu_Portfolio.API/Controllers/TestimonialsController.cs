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
            var testimonials = await _context.Testimonials
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(ApiResponse<List<Testimonial>>.SuccessResponse(
                testimonials,
                "Testimonials retrieved successfully"
            ));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);

            if (testimonial == null)
            {
                return NotFound(ApiResponse<object>.FailResponse("Testimonial not found"));
            }

            return Ok(ApiResponse<Testimonial>.SuccessResponse(
                testimonial,
                "Testimonial retrieved successfully"
            ));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTestimonialDto dto)
        {
            try
            {
                var testimonial = new Testimonial
                {
                    ClientName = dto.ClientName,
                    Company = dto.Company,
                    Comment = dto.Comment,
                    Rating = dto.Rating,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Testimonials.Add(testimonial);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<Testimonial>.SuccessResponse(
                    testimonial,
                    "Testimonial created successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "Failed to create testimonial",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTestimonialDto dto)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);

            if (testimonial == null)
            {
                return NotFound(ApiResponse<object>.FailResponse("Testimonial not found"));
            }

            testimonial.ClientName = dto.ClientName;
            testimonial.Company = dto.Company;
            testimonial.Comment = dto.Comment;
            testimonial.Rating = dto.Rating;
            testimonial.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Testimonial>.SuccessResponse(
                testimonial,
                "Testimonial updated successfully"
            ));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);

            if (testimonial == null)
            {
                return NotFound(ApiResponse<object>.FailResponse("Testimonial not found"));
            }

            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                null!,
                "Testimonial deleted successfully"
            ));
        }
    }
}