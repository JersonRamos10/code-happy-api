using System.Security.Claims;
using codeHappy.Business.Dtos;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Models;

namespace codeHappy.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth").RequireAuthorization();


        group.MapPost("/sync", async (HttpContext context, IProfileService profileService) =>
        {
            // Extrae los claims del JWT emitido por Supabase
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = context.User.FindFirstValue(ClaimTypes.Email);
            var userName = context.User.FindFirstValue(ClaimTypes.Name);
            var displayName = context.User.FindFirstValue("display_name");


            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email)
                || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(displayName))
                return Results.Unauthorized();

            var guidUserId = Guid.Parse(userId);

            await profileService.SyncProfileAsync(guidUserId, email, userName, displayName);

            var profile = await profileService.GetUserbyIdAsync(guidUserId);

            return Results.Ok(profile);
        });
    }
}