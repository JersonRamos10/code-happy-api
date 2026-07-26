using codeHappy.Business.Dtos.Blocks;
using codeHappy.Business.Interfaces;
using FluentValidation;

namespace codeHappy.Api.Endpoints;

public static class BlockEndpoints
{
    public static void MapBlockEndpoints(this WebApplication app)
    {
        var blocks = app.MapGroup("api/snippets/{snippetId}/blocks").RequireAuthorization();

        // POST /snippets/{snippetId}/blocks — creates a block.
        blocks.MapPost("/", async (
            Guid snippetId,
            CreateBlockRequest request,
            IValidator<CreateBlockRequest> validator,
            IBlocksService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(request, ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var block = await service.CreateBlockAsync(Guid.Parse(userId), snippetId, request, ct);

            return Results.Created($"/snippets/{snippetId}/blocks/{block.Id}", block);
        });

        // PUT /snippets/{snippetId}/blocks/{blockId} — updates title/content/language (PATCH-like, null fields are left untouched).
        blocks.MapPut("/{blockId}", async (
            Guid snippetId,
            Guid blockId,
            UpdateBlockRequest request,
            IValidator<UpdateBlockRequest> validator,
            IBlocksService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(request, ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.UpdateBlockContentAsync(Guid.Parse(userId), snippetId, blockId, request, ct);

            return Results.NoContent();
        });

        // PUT /snippets/{snippetId}/blocks/{blockId}/annotations — replaces annotations. A null/empty body clears them.
        blocks.MapPut("/{blockId}/annotations", async (
            Guid snippetId,
            Guid blockId,
            List<CreateAnnotationRequest>? request,
            IValidator<List<CreateAnnotationRequest>> validator,
            IBlocksService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            if (request is not null)
            {
                var result = await validator.ValidateAsync(request, ct);

                if (!result.IsValid)
                    return Results.ValidationProblem(result.ToDictionary());
            }

            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.UpdateBlockAnnotations(Guid.Parse(userId), snippetId, blockId, request, ct);

            return Results.NoContent();
        });

        // DELETE /snippets/{snippetId}/blocks/{blockId} — deletes the block.
        blocks.MapDelete("/{blockId}", async (
            Guid snippetId,
            Guid blockId,
            IBlocksService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.DeleteBlock(Guid.Parse(userId), snippetId, blockId, ct);

            return Results.NoContent();
        });

        // PUT /snippets/{snippetId}/blocks/reorder — bulk-updates block positions.
        blocks.MapPut("/reorder", async (
            Guid snippetId,
            List<ReorderBlockRequest> request,
            IValidator<List<ReorderBlockRequest>> validator,
            IBlocksService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(request ?? [], ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.ReorderBlocks(Guid.Parse(userId), snippetId, request, ct);

            return Results.NoContent();
        });


    }
}
