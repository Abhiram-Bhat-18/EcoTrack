
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcoTrack.API.Data;
using EcoTrack.API.DTOs;
using EcoTrack.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcoTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthController(
        AppDbContext context,
        IPasswordHasher<AppUser> passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            return Conflict(new
            {
                message = "An account with this email already exists."
            });
        }

        var user = new AppUser
        {
            Email = email,
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user, request.Password);

        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Registration could not be completed. The email may already be registered."
            });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Registration successful. You can now log in."
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();


        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Email == email);

        if (user is null || !user.IsActive)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var verificationResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        if (verificationResult ==
            PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(
                user, request.Password);

            await _context.SaveChangesAsync();
        }


        var jwt = _configuration.GetSection("Jwt");
        var keyValue = jwt["Key"];

        if (string.IsNullOrWhiteSpace(keyValue))
        {
            return Problem(
                statusCode: 500,
                title: "Authentication is not configured.");
        }

        byte[] keyBytes;

        try
        {
            keyBytes = Convert.FromBase64String(keyValue);
        }
        catch (FormatException)
        {
            return Problem(
                statusCode: 500,
                title: "The JWT signing key is incorrectly configured.");
        }

        if (keyBytes.Length < 32)
        {
            return Problem(
                statusCode: 500,
                title: "The JWT signing key is too short.");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(30);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Ok(new
        {
            message = "Login successful.",
            token = tokenString,
            expiresAt,
            user = new
            {
                user.Id,
                user.Email,
                user.Role
            }
        });
    }
}
