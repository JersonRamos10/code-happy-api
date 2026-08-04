using codeHappy.Business.Dtos.Snippet;
using codeHappy.Business.Interfaces;
using FluentValidation;

namespace codeHappy.Api.Endpoints;

public static class SnippetEndpoints
{
    public static void MapSnippetEndpoints(this WebApplication app)
    {

        var groups = app.MapGroup("api/snippets").RequireAuthorization();

        groups.MapGet("/", async (
             ISnippetService service,
             ICurrentUserService current,
            [AsParameters] SnippetParamsRequest req,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            var snippets = await service.GetAllSnippetAsync(Guid.Parse(userId), req, ct);

            return Results.Ok(snippets);

        });

        groups.MapGet("/{id}", async (Guid id, ISnippetService service, ICurrentUserService current, CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            var snippet = await service.GetSnippetByIdAsync(Guid.Parse(userId), id, ct);

            return Results.Ok(snippet);
        });

        groups.MapPost("/", async (
            CreateSnippetRequest request,
            IValidator<CreateSnippetRequest> validator,
            ISnippetService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(request, ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            var snippet = await service.CreateSnippetAsync(Guid.Parse(userId), request, ct);

            return Results.Created($"/snippets/{snippet.Id}", snippet);
        });

        groups.MapPut("/{id}", async (
            Guid id,
            UpdateSnippetRequest request,
            IValidator<UpdateSnippetRequest> validator,
            ISnippetService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(request, ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            await service.UpdateSnippetWithBlocksAsync(id, Guid.Parse(userId), request, ct);

            return Results.NoContent();
        });

        groups.MapDelete("/{id}", async (
            Guid id,
            ISnippetService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            await service.DeleteSnippetbyId(Guid.Parse(userId), id, ct);

            return Results.NoContent();
        });

        groups.MapPatch("/{id}/favorite", async (
            Guid id,
            ISnippetService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            await service.ToggleFavorite(Guid.Parse(userId), id, ct);

            return Results.NoContent();
        });

        groups.MapPost("/{id}/copy", async (
            Guid id,
            ISnippetService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            await service.RecordCopy(Guid.Parse(userId), id, ct);

            return Results.NoContent();
        });

        groups.MapPatch("/{id}/move", async (
            Guid id,
            MoveSnippetRequest? request,
            IValidator<MoveSnippetRequest> validator,
            ISnippetService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            if (request is null)
                return Results.BadRequest();

            var result = await validator.ValidateAsync(request, ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (userId is null)
                return Results.Unauthorized();

            await service.MoveSnippetAsync(Guid.Parse(userId), id, request, ct);

            return Results.NoContent();
        });
    }
}
