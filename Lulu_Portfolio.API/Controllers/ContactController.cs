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
            var messages = await _context.ContactMessages
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(ApiResponse<List<ContactMessage>>.SuccessResponse(
                messages,
                "Contact messages retrieved successfully"
            ));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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

        [HttpPost]
        public async Task<IActionResult> Create(CreateContactMessageDto dto)
        {
            try
            {
                var message = new ContactMessage
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Subject = dto.Subject,
                    Message = dto.Message,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ContactMessages.Add(message);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<ContactMessage>.SuccessResponse(
                    message,
                    "Message sent successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    "Failed to send message",
                    new List<string> { ex.Message }
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
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
    }
}