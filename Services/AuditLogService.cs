
using EcoTrack.API.Data;
using EcoTrack.API.Models;

namespace EcoTrack.API.Services;

public class AuditLogService
{
    private readonly AppDbContext _context;

    public AuditLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(
        int? actorUserId,
        string actorEmail,
        string action,
        string entityType,
        string? entityId = null,
        string? details = null)
    {
        var log = new AuditLog
        {
            ActorUserId = actorUserId,
            ActorEmail = actorEmail,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
