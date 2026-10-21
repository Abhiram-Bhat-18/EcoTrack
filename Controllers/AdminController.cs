
using EcoTrack.API.Data;
using EcoTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EcoTrack.API.Services;

namespace EcoTrack.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "ActiveUser", Roles = "Admin")]
public class AdminController : ControllerBase
{

    private readonly AppDbContext _context;
    private readonly AuditLogService _auditLog;

    public AdminController(
        AppDbContext context,
        AuditLogService auditLog)
    {
        _context = context;
        _auditLog = auditLog;
    }


    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var totalUsers = await _context.Users.CountAsync();

        var totalReports = await _context.WasteReports.CountAsync();

        var pendingReports = await _context.WasteReports
            .CountAsync(report => report.Status == "Pending");

        var collectedReports = await _context.WasteReports
            .CountAsync(report => report.Status == "Collected");

        var resolvedReports = await _context.WasteReports
            .CountAsync(report => report.Status == "Resolved");


        return Ok(new
        {
            totalUsers,
            totalReports,
            pendingReports,
            collectedReports,
            resolvedReports
        });
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetAllReports()
    {
        var reports = await _context.WasteReports
            .AsNoTracking()
            .OrderByDescending(report => report.ReportedAt)
            .Select(report => new
            {
                report.Id,
                report.WasteType,
                report.Description,
                report.Location,
                report.Status,
                report.UserId,
                report.ReportedAt
            })
            .ToListAsync();

        return Ok(reports);
    }

    [HttpPut("reports/{id:int}/status")]
    public async Task<IActionResult> UpdateReportStatus(
        int id,
        [FromBody] string status)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid report ID."
            });
        }

        if (status is not ("Pending" or "Collected" or "Resolved"))
        {
            return BadRequest(new
            {
                message = "Status must be Pending, Collected, or Resolved."
            });
        }

        var report = await _context.WasteReports.FindAsync(id);

        if (report is null)
        {
            return NotFound(new
            {
                message = "Waste report not found."
            });
        }

     
        var newStatus = status.Trim();

        if (newStatus.Equals("Collected",
                StringComparison.OrdinalIgnoreCase)
            && report.CollectedAt == null)
        {
            report.CollectedAt = DateTime.UtcNow;
        }

        if (newStatus.Equals("Resolved",
                StringComparison.OrdinalIgnoreCase)
            && report.ResolvedAt == null)
        {
            report.ResolvedAt = DateTime.UtcNow;
        }

        report.Status = newStatus;

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
            "ReportStatusChanged",
            "WasteReport",
            id.ToString(),
            $"Report status changed to {report.Status}");

        return Ok(new
        {
            message = "Report status updated successfully."
        });
    }
}
