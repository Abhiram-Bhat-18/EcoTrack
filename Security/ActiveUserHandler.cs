
using System.Security.Claims;
using EcoTrack.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.API.Security;

public sealed class ActiveUserHandler
    : AuthorizationHandler<ActiveUserRequirement>
{
    private readonly AppDbContext _context;

    public ActiveUserHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ActiveUserRequirement requirement)
    {
        var userIdText =
            context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdText, out var userId))
            return;

        var isActive = await _context.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId && user.IsActive);

        if (isActive)
        {
            context.Succeed(requirement);
        }
    }
}
