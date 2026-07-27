using codeHappy.Business.Dtos.Groups;
using codeHappy.Business.Interfaces;
using FluentValidation;

namespace codeHappy.Api.Endpoints;

public static class GroupEndpoints
{

    public static void MapGroupEndpoints(this WebApplication app)
    {
        var groups = app.MapGroup("api/spaces/{spaceId}/groups").RequireAuthorization();

        // GET /spaces/{spaceId}/groups — returns all groups in the space ordered by position.
        groups.MapGet("/", async (
            Guid spaceId,
            ICurrentUserService current,
            IGroupService service,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var groups = await service.GetAllGroupsAsync(spaceId, Guid.Parse(userId), ct);

            return Results.Ok(groups);
        });

        // POST /spaces/{spaceId}/groups — creates a group. Position is calculated automatically.
        groups.MapPost("/", async (
            Guid spaceId,
            CreateGroupRequest request,
            IGroupService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var group = await service.CreateGroupAsync(spaceId, Guid.Parse(userId), request.Name, ct);

            return Results.Created($"api/spaces/{spaceId}/groups/{group.Id}", group);
        });

        // PUT /spaces/{spaceId}/groups/{id} — renames the group.
        groups.MapPut("/{id}", async (
            Guid spaceId,
            Guid id,
            UpdateGroupRequest request,
            IGroupService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.UpdateGroupAsync(id, Guid.Parse(userId), request.Name, ct);

            return Results.NoContent();
        });

        // DELETE /spaces/{spaceId}/groups/{id} — deletes the group.
        groups.MapDelete("/{id}", async (
            Guid spaceId,
            Guid id,
            IGroupService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.DeleteGroupAsync(id, Guid.Parse(userId), ct);

            return Results.NoContent();
        });

        // PUT /spaces/{spaceId}/groups/reorder — bulk-updates group positions.
        groups.MapPut("/reorder", async (
            Guid spaceId,
            List<ReorderGroupRequest> request,
            IValidator<List<ReorderGroupRequest>> validator,
            IGroupService service,
            ICurrentUserService current,
            CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(request ?? [], ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.ReorderGroupsAsync(spaceId, Guid.Parse(userId), request, ct);

            return Results.NoContent();
        });
    }
}