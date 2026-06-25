using System.Security.Claims;
using codeHappy.Api.Dtos;
using codeHappy.Data.Context;
using codeHappy.Data.Models;

namespace codeHappy.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth").RequireAuthorization();

        group.MapPost("/sync", async (HttpContext context, CodeHappyContext db) =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = context.User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var guidUserId = Guid.Parse(userId);

            var profile = await db.Profiles.FindAsync(guidUserId);

            if (profile == null)
            {
                profile = new Profile
                {
                    Id = guidUserId,
                    UserName = $"user_{guidUserId.ToString().Substring(0, 8)}",
                    Email = email ?? "",
                    DisplayName = context.User.FindFirstValue(ClaimTypes.Name) ?? "Nuevo Usuario"
                };

                db.Profiles.Add(profile);
                await db.SaveChangesAsync();
            }

            return Results.Ok(new UserProfileResponse(
                profile.Id,
                profile.UserName,
                profile.Email,
                profile.AvatarUrl
            ));
        });
    }
}