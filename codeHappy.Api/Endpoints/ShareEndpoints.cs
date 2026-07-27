using codeHappy.Business.Dtos.Shares;
using codeHappy.Business.Interfaces;
using FluentValidation;

namespace codeHappy.Api.Endpoints;

public static class ShareEndpoints
{
    public static void MapShareEndpoints(this WebApplication app)
    {
        var snippetShares = app.MapGroup("api/snippets/{snippetId}/shares").RequireAuthorization();

        // POST /snippets/{snippetId}/shares — creates a share for the snippet.
        snippetShares.MapPost("/", async (
            Guid snippetId,
            CreateShareRequest request,
            IValidator<CreateShareRequest> validator,
            IShareService service,
            ICurrentUserService current) =>
        {
            var normalizedRequest = request with { SnippetId = snippetId };

            var result = await validator.ValidateAsync(normalizedRequest);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var share = await service.CreateShareAsync(Guid.Parse(userId), normalizedRequest);

            return Results.Created($"api/shared/{share.Id}", share);
        });

        var shares = app.MapGroup("api/shares").RequireAuthorization();

        // GET /shares — lists the shares created by the authenticated user.
        shares.MapGet("/", async (
            IShareService service,
            ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var result = await service.GetMySharesAsync(Guid.Parse(userId));

            return Results.Ok(result);
        });

        // DELETE /shares/{shareId} — revokes (deletes) a share.
        shares.MapDelete("/{shareId}", async (
            Guid shareId,
            IShareService service,
            ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.DeleteShareAsync(Guid.Parse(userId), shareId);

            return Results.NoContent();
        });

        // GET /shared/{shareId} — anonymous, public view of the shared snippet. Intentionally NOT under RequireAuthorization():
        // this is the one link meant to work without a logged-in user, so it lives in its own MapGroup to make that explicit.
        var publicShares = app.MapGroup("api/shared");

        publicShares.MapGet("/{shareId}", async (
            Guid shareId,
            IShareService service) =>
        {
            var snippet = await service.GetSharedSnippetAsync(shareId);

            return Results.Ok(snippet);
        });
    }
}