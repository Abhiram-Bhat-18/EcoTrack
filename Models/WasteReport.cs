
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTrack.API.Models;

public class WasteReport
{

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public DateTime? CollectedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public int Id { get; set; }

    [Required]
    [RegularExpression("^(Plastic|Organic|Paper|Electronic|Mixed|Other)$")]
    [StringLength(100)]
    public string WasteType { get; set; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 3)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Location { get; set; } = string.Empty;

    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [RegularExpression("^(Pending|Collected|Resolved)$")]
    [StringLength(30)]
    public string Status { get; set; } = "Pending";

    // Nullable so existing reports can remain in the database
    // until they are assigned to a user.
    public int? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser? User { get; set; }
}
