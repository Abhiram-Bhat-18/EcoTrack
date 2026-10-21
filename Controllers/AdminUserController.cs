
using System.Security.Claims;
using EcoTrack.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoTrack.API.Services;
namespace EcoTrack.API.Controllers;


[ApiController]
[Route("api/admin/users")]
[Authorize(Policy = "ActiveUser", Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuditLogService _auditLog;
    public AdminUsersController(
     AppDbContext context,
     AuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }

    private int CurrentAdminId =>
        int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var id) ? id : 0;

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .AsNoTracking()
            .OrderBy(user => user.Email)
            .Select(user => new
            {
                user.Id,
                user.Email,
                user.Role,
                user.CreatedAt,
                user.IsActive
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPut("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(
        int id,
        [FromBody] UpdateRoleRequest request)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid user ID." });

        if (request.Role is not ("User" or "Admin"))
            return BadRequest(new { message = "Role must be User or Admin." });

        var user = await _context.Users.FindAsync(id);

        if (user is null)
            return NotFound(new { message = "User not found." });

        if (!user.IsActive && request.Role == "Admin")
            return BadRequest(new { message = "Activate the account before promoting it." });

        if (user.Id == CurrentAdminId && request.Role != "Admin")
            return BadRequest(new { message = "You cannot demote your own account." });

        if (user.Role == "Admin" && request.Role != "Admin")
        {
            var activeAdmins = await _context.Users
                .CountAsync(u => u.Role == "Admin" && u.IsActive);

            if (activeAdmins <= 1)
                return BadRequest(new { message = "The last active administrator cannot be demoted." });
        }

        user.Role = request.Role;
        await _context.SaveChangesAsync();

        var actorIdText = User.FindFirstValue(
    ClaimTypes.NameIdentifier);

        int? actorId = int.TryParse(actorIdText, out var parsedId)
            ? parsedId
            : null;

        var actorEmail = User.FindFirstValue(ClaimTypes.Email)
            ?? "Unknown";

        await _auditLog.LogAsync(
            actorId,
            actorEmail,
            "UserRoleChanged",
            "User",
            id.ToString(),
            $"Role changed to {request.Role}");
        return Ok(new { message = "User role updated successfully." });
        
    }

    [HttpPut("{id:int}/active")]
    public async Task<IActionResult> UpdateActiveStatus(
        int id,
        [FromBody] UpdateActiveRequest request)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid user ID." });

        var user = await _context.Users.FindAsync(id);

        if (user is null)
            return NotFound(new { message = "User not found." });

        if (user.Id == CurrentAdminId && !request.IsActive)
            return BadRequest(new { message = "You cannot deactivate your own account." });

        if (user.Role == "Admin" && !request.IsActive)
        {
            var activeAdmins = await _context.Users
                .CountAsync(u => u.Role == "Admin" && u.IsActive);

            if (activeAdmins <= 1)
                return BadRequest(new { message = "The last active administrator cannot be deactivated." });
        }

        user.IsActive = request.IsActive;
        await _context.SaveChangesAsync();
        var actorIdText = User.FindFirstValue(
    ClaimTypes.NameIdentifier);

        int? actorId = int.TryParse(actorIdText, out var parsedId)
            ? parsedId
            : null;

        var actorEmail = User.FindFirstValue(ClaimTypes.Email)
            ?? "Unknown";

        await _auditLog.LogAsync(
            actorId,
            actorEmail,
            "UserActiveStatusChanged",
            "User",
            id.ToString(),
            $"User active status changed to {request.IsActive}");
        return Ok(new { message = "User account status updated successfully." });
    }
}

public class UpdateRoleRequest
{
    public string Role { get; set; } = string.Empty;
}

public class UpdateActiveRequest
{
    public bool IsActive { get; set; }
}
