using codeHappy.Business.Dtos.Snippet;
using codeHappy.Business.Interfaces;

namespace codeHappy.Api.Endpoints;

public static class SnippetEndpoints
{
    public static void MapSnippetEndpoints(this WebApplication app)
    {

        var groups = app.MapGroup("/snippets").RequireAuthorization();

        groups.MapGet("/", async (
             ISnippetService service,
             ICurrentUserService current,
            [AsParameters] SnippetParamsRequest req) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            var snippets = await service.GetAllSnippetAsync(Guid.Parse(userId), req);

            return Results.Ok(snippets);

        });

        groups.MapGet("/{id}", async (string id, ISnippetService service, ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            var snippet = await service.GetSnippetByIdAsync(Guid.Parse(userId), Guid.Parse(id));

            return Results.Ok(snippet);
        });

        groups.MapPost("/", async (
            CreateSnippetRequest request,
            ISnippetService service,
            ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            var snippet = await service.CreateSnippetAsync(Guid.Parse(userId), request);

            return Results.Created($"/snippets/{snippet.Id}", snippet);
        });

        groups.MapPut("/{id}", async (
            string id,
            UpdateSnippetRequest request,
            ISnippetService service,
            ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            await service.UpdateSnippetWithBlocksAsync(Guid.Parse(id), Guid.Parse(userId), request);

            return Results.NoContent();
        });

        groups.MapDelete("/{id}", async (
            string id,
            ISnippetService service,
            ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            await service.DeleteSnippetbyId(Guid.Parse(userId), Guid.Parse(id));

            return Results.NoContent();
        });

        groups.MapPatch("/{id}/favorite", async (
            string id,
            ISnippetService service) =>
        {
            await service.ToggleFavorite(Guid.Parse(id));

            return Results.NoContent();
        });

        groups.MapPost("/{id}/copy", async (
            string id,
            ISnippetService service) =>
        {
            await service.RecordCopy(Guid.Parse(id));

            return Results.NoContent();
        });
    }
}
