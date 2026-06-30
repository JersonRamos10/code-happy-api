using codeHappy.Business.Dtos.Group;
using codeHappy.Business.Interfaces;

namespace codeHappy.Api.Endpoints;

public static class GroupEndpoints
{

    public static void MapGroupEndpoints(this WebApplication app)
    {
        var groups = app.MapGroup("/spaces/{spaceId}/groups").RequireAuthorization();

        // GET /spaces/{spaceId}/groups — returns all groups in the space ordered by position.
        groups.MapGet("/", async (
            Guid spaceId,
            ICurrentUserService current,
            IGroupService service) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var groups = await service.GetAllGroupsAsync(spaceId, Guid.Parse(userId));

            return Results.Ok(groups);
        });

        // POST /spaces/{spaceId}/groups — creates a group. Position is calculated automatically.
        groups.MapPost("/", async (
            Guid spaceId,
            CreateGroupRequest request,
            IGroupService service,
            ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var group = await service.CreateGroupAsync(spaceId, Guid.Parse(userId), request.Name);

            return Results.Created($"/spaces/{spaceId}/groups/{group.Id}", group);
        });

        // PUT /spaces/{spaceId}/groups/{id} — renames the group.
        groups.MapPut("/{id}", async (
            Guid spaceId,
            Guid id,
            UpdateGroupRequest request,
            IGroupService service,
            ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.UpdateGroupAsync(id, Guid.Parse(userId), request.Name);

            return Results.NoContent();
        });

        // DELETE /spaces/{spaceId}/groups/{id} — deletes the group.
        groups.MapDelete("/{id}", async (
            Guid spaceId,
            Guid id,
            IGroupService service,
            ICurrentUserService current) =>
        {
            var userId = current.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await service.DeleteGroupAsync(id, Guid.Parse(userId));

            return Results.NoContent();
        });
    }
}