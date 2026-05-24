using Lulu_Portfolio.API.Models;
using Lulu_Portfolio.API.Models.Auth;
using Lulu_Portfolio.API.Services;
using Lulu_Portfolio.API.Services.Interfaces;
using Lulu_Portfolio.Domain.Entities;
using Lulu_Portfolio.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lulu_Portfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("Email and password are required"));
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.Email == request.Email);

                if (user == null)
                {
                    return Unauthorized(ApiResponse<object>.FailResponse("Invalid email or password"));
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(ApiResponse<object>.FailResponse("Invalid email or password"));
                }

                var token = _jwtService.GenerateToken(user.Email!, user.Role);

                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    data = new
                    {
                        token,
                        email = user.Email,
                        role = user.Role,
                        fullName = user.FullName,
                        id = user.Id
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Login failed",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(ApiResponse<object>.FailResponse("All fields are required"));
                }

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(x => x.Email == request.Email);

                if (existingUser != null)
                {
                    return BadRequest(ApiResponse<object>.FailResponse("User already exists with this email"));
                }

                var user = new User
                {
                    FullName = request.FullName.Trim(),
                    Email = request.Email.Trim().ToLower(),
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };

                var hasher = new PasswordHasher<User>();
                user.PasswordHash = hasher.HashPassword(user, request.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "User registered successfully",
                    data = new
                    {
                        user.Id,
                        user.Email,
                        user.FullName,
                        user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Registration failed",
                    new List<string> { ex.Message }
                ));
            }
        }
    }
}

//dotnet  watch run --project Lulu_Portfolio.API  