
namespace EcoTrack.API.Models;

public class AuditLog
{
    public int Id { get; set; }

    public int? ActorUserId { get; set; }

    public string ActorEmail { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public string? Details { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
