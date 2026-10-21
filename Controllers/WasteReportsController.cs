
using System.Security.Claims;
using EcoTrack.API.Data;
using EcoTrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WasteReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DateTime? CollectedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }
    public WasteReportsController(AppDbContext context)
    {
        _context = context;
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdValue, out userId) && userId > 0;
    }

    // GET: api/WasteReports
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WasteReport>>> GetReports()
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        var reports = await _context.WasteReports
            .AsNoTracking()
            .Where(report => report.UserId == userId)
            .OrderByDescending(report => report.ReportedAt)
            .ToListAsync();

        return Ok(reports);
    }

    // GET: api/WasteReports/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WasteReport>> GetReport(int id)
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        if (id <= 0)
            return BadRequest(new { message = "Report ID must be positive." });

        var report = await _context.WasteReports
            .AsNoTracking()
            .FirstOrDefaultAsync(report =>
                report.Id == id && report.UserId == userId);

        if (report is null)
            return NotFound(new { message = "Waste report was not found." });

        return Ok(report);
    }

    // POST: api/WasteReports
    [HttpPost]
    public async Task<ActionResult<WasteReport>> CreateReport(
        [FromBody] WasteReport report)
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        // Never trust ownership or system fields supplied by the client.
        report.Id = 0;
        report.UserId = userId;
        report.User = null;
        report.ReportedAt = DateTime.UtcNow;
        report.Status = "Pending";

        // Set system-managed timestamps on the server.
        report.CollectedAt = null;
        report.ResolvedAt = null;

        _context.WasteReports.Add(report);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Could not save the waste report.",
                detail: "Please try again.");
        }

        return CreatedAtAction(
            nameof(GetReport),
            new { id = report.Id },
            report);
    }

    // PUT: api/WasteReports/5/status
    // Request body: "Collected"
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] string status)
    {
        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        if (id <= 0)
            return BadRequest(new { message = "Report ID must be positive." });

        string[] allowedStatuses = { "Pending", "Collected", "Resolved" };

        if (string.IsNullOrWhiteSpace(status) ||
            !allowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Invalid status. Choose Pending, Collected, or Resolved."
            });
        }

        var report = await _context.WasteReports
            .FirstOrDefaultAsync(report =>
                report.Id == id && report.UserId == userId);

        if (report is null)
            return NotFound(new { message = "Waste report was not found." });


        var newStatus = allowedStatuses.First(
            allowedStatus => allowedStatus.Equals(
                status, StringComparison.OrdinalIgnoreCase));

        if (newStatus == "Collected" && report.CollectedAt == null)
        {
            report.CollectedAt = DateTime.UtcNow;
        }

        if (newStatus == "Resolved" && report.ResolvedAt == null)
        {
            report.ResolvedAt = DateTime.UtcNow;
        }

        report.Status = newStatus;


        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Could not update the report status.",
                detail: "Please try again.");
        }

        return Ok(new
        {
            message = "Status updated successfully.",
            report.Id,
            report.Status
        });
    }
}
