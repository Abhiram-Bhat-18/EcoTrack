
using EcoTrack.API.Models;
using Microsoft.EntityFrameworkCore;
namespace EcoTrack.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<WasteReport> WasteReports => Set<WasteReport>();

    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .Property(user => user.Email)
            .HasMaxLength(256)
            .IsRequired();

        modelBuilder.Entity<AppUser>()
            .Property(user => user.PasswordHash)
            .IsRequired();

        modelBuilder.Entity<AppUser>()
            .Property(user => user.Role)
            .HasMaxLength(20)
            .HasDefaultValue("User")
            .IsRequired();

        modelBuilder.Entity<AppUser>()
            .Property(user => user.IsActive)
            .HasDefaultValue(true)
            .IsRequired();
    }
}
