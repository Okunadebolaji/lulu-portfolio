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
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var messages = await _context.ContactMessages
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return Ok(ApiResponse<List<ContactMessage>>.SuccessResponse(
                    messages,
                    "Contact messages retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to retrieve messages",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var message = await _context.ContactMessages.FindAsync(id);

                if (message == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Message not found"));
                }

                return Ok(ApiResponse<ContactMessage>.SuccessResponse(
                    message,
                    "Message retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to retrieve message",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactMessageDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.FullName))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("Full name is required"));
                }

                if (string.IsNullOrWhiteSpace(dto.Email))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("Email is required"));
                }

                if (string.IsNullOrWhiteSpace(dto.Message))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("Message is required"));
                }

                var message = new ContactMessage
                {
                    FullName = dto.FullName.Trim(),
                    Email = dto.Email.Trim(),
                    Subject = dto.Subject?.Trim() ?? string.Empty,
                    Message = dto.Message.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.ContactMessages.Add(message);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Message sent successfully",
                    data = message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to send message",
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
                var message = await _context.ContactMessages.FindAsync(id);

                if (message == null)
                {
                    return NotFound(ApiResponse<object>.FailResponse("Message not found"));
                }

                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.SuccessResponse(
                    null!,
                    "Message deleted successfully"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to delete message",
                    new List<string> { ex.Message }
                ));
            }
        }
    }
}