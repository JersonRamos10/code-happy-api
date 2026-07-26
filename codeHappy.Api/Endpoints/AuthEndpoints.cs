using System.Security.Claims;
using codeHappy.Business.Dtos;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Models;
using FluentValidation.AspNetCore;

namespace codeHappy.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/auth").RequireAuthorization();


        // POST /auth/sync — syncs the Supabase JWT claims into the local profiles table. Creates the profile if it doesn't exist.
        group.MapPost("/sync", async (ICurrentUserService current, IProfileService profileService, CancellationToken ct) =>
        {
            var userId = current.GetUserId();
            var email = current.GetEmail();
            var userName = current.GetUserName();
            var displayName = current.GetDisplayName();


            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email)
                || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(displayName))
                return Results.Unauthorized();

            var guidUserId = Guid.Parse(userId);

            await profileService.SyncProfileAsync(guidUserId, email, userName, displayName, ct);

            var profile = await profileService.GetUserbyIdAsync(guidUserId, ct);

            return Results.Ok(profile);
        });
    }
}