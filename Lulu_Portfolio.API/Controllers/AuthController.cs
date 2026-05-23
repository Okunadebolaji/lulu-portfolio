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
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(ApiResponse<object>.FailResponse(
                    "Invalid email or password"
                ));
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(ApiResponse<object>.FailResponse(
                    "Invalid email or password"
                ));
            }

            var token = _jwtService.GenerateToken(
                user.Email,
                user.Role
            );

            return Ok(ApiResponse<object>.SuccessResponse(
                new
                {
                    token,
                    email = user.Email,
                    role = user.Role,
                    fullName = user.FullName
                },
                "Login successful"
            ));
        }
        
[HttpPost("register")]
public async Task<IActionResult> Register(RegisterRequest request)
{
    var existingUser = await _context.Users
        .FirstOrDefaultAsync(x => x.Email == request.Email);

    if (existingUser != null)
    {
        return BadRequest(ApiResponse<object>.FailResponse(
            "User already exists with this email"
        ));
    }

    var user = new User
    {
        FullName = request.FullName!,
        Email = request.Email!,
        // ✅ Default new users to "User" role
        // Only change to "Admin" manually in database or via admin panel
        Role = "User",
        CreatedAt = DateTime.UtcNow
    };

    var hasher = new PasswordHasher<User>();
    user.PasswordHash = hasher.HashPassword(user, request.Password!);

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok(ApiResponse<object>.SuccessResponse(
        new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Role
        },
        "User registered successfully"
    ));
}
 }
}

//dotnet  watch run --project Lulu_Portfolio.API  