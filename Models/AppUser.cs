
using System.ComponentModel.DataAnnotations;

namespace EcoTrack.API.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;
}
