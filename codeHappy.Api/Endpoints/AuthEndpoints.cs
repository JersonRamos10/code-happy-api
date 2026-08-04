using System.Security.Claims;
using codeHappy.Business.Dtos;
using codeHappy.Business.Dtos.Profile;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace codeHappy.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/auth").RequireAuthorization();


        // POST /auth/sync — syncs the Supabase JWT claims into the local profiles table. Creates the profile if it doesn't exist.
        group.MapPost("/sync", async (
            ICurrentUserService current,
            [FromBody] SyncProfileRequest request,
            IValidator<SyncProfileRequest> validator,
            IProfileService profileService,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();
            var email = current.GetEmail();

            var result = await validator.ValidateAsync(request, ct);


            if(!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email)
                || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.DisplayName))
                return Results.Unauthorized();

            if(!Guid.TryParse(userId , out  var guidUserId))
                    return Results.Unauthorized();

            await profileService.SyncProfileAsync(guidUserId, email, request.UserName, request.DisplayName, ct);

            var profile = await profileService.GetUserbyIdAsync(guidUserId, ct);

            return Results.Ok(profile);
        });
    }
}