
using EcoTrack.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.API.Controllers;

[ApiController]
[Route("api/admin/audit-logs")]
[Authorize(Policy = "ActiveUser", Roles = "Admin")]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditLogsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(log => log.CreatedAt);

        var total = await query.CountAsync();

        var logs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(log => new
            {
                log.Id,
                log.ActorUserId,
                log.ActorEmail,
                log.Action,
                log.EntityType,
                log.EntityId,
                log.Details,
                log.CreatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            logs
        });
    }
}
