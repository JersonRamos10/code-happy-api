
using Microsoft.EntityFrameworkCore;
using codeHappy.Business.Dtos.Groups;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Models;


namespace codeHappy.Business.Services;

public class GroupService(CodeHappyContext context) : IGroupService
{
    // Creates a group inside a space. Position is calculated as the current group count in that space.
    public async Task<GroupResponse> CreateGroupAsync(Guid spaceId, Guid userId, string name, CancellationToken ct)
    {
        var space = await context.Spaces
                .Include(s => s.Groups)
                .FirstOrDefaultAsync(s => s.Id == spaceId, ct)
                ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        var group = new Group
        {
            Name = name,
            SpaceId = spaceId,
            Position = space.Groups.Count,
        };

        await context.Groups.AddAsync(group, ct);
        await context.SaveChangesAsync(ct);

        return MapToResponse(group);
    }

    // Returns all groups in a space ordered by position ascending.
    public async Task<IEnumerable<GroupResponse>> GetAllGroupsAsync(Guid spaceId, Guid userId, CancellationToken ct)
    {
        var space = await context.Spaces
            .FirstOrDefaultAsync(s => s.Id == spaceId, ct)
            ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        return await context.Groups
                .Where(g => g.SpaceId == spaceId)
                .OrderBy(g => g.Position)
                .Select(g => MapToResponse(g))
                .ToListAsync(ct);
    }

    // Returns one group only when it belongs to the requested space and the user owns that space.
    public async Task<GroupResponse> GetGroupAsync(Guid spaceId, Guid groupId, Guid userId, CancellationToken ct)
    {
        var space = await context.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == spaceId, ct)
            ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        var group = await context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == groupId && g.SpaceId == spaceId, ct)
            ?? throw new NotFoundException("Group", groupId);

        return MapToResponse(group);
    }

    // Renames the group. Scoped by the route's space so a crossed spaceId cannot reach it.
    public async Task UpdateGroupAsync(Guid spaceId, Guid groupId, Guid userId, string name, CancellationToken ct)
    {
        var space = await context.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == spaceId, ct)
            ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        var group = await context.Groups
            .FirstOrDefaultAsync(g => g.Id == groupId && g.SpaceId == spaceId, ct)
            ?? throw new NotFoundException("Group", groupId);

        group.Name = name;
        group.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
    }

    // Deletes the group. Same route-scoping rule as the update.
    public async Task DeleteGroupAsync(Guid spaceId, Guid groupId, Guid userId, CancellationToken ct)
    {
        var space = await context.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == spaceId, ct)
            ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        var group = await context.Groups
            .FirstOrDefaultAsync(g => g.Id == groupId && g.SpaceId == spaceId, ct)
            ?? throw new NotFoundException("Group", groupId);

        context.Groups.Remove(group);
        await context.SaveChangesAsync(ct);
    }

    // Reorders groups within a space. The client sends the final positions — the service only persists them.
    public async Task ReorderGroupsAsync(Guid spaceId, Guid userId, List<ReorderGroupRequest> reorderGroups, CancellationToken ct)
    {
        var space = await context.Spaces
            .Include(s => s.Groups)
            .FirstOrDefaultAsync(s => s.Id == spaceId, ct)
            ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        foreach (var item in reorderGroups)
        {
            var group = space.Groups
                .FirstOrDefault(g => g.Id == item.Id)
                ?? throw new NotFoundException("Group", item.Id);

            group.Position = item.Position;
        }

        await context.SaveChangesAsync(ct);
    }

    private static GroupResponse MapToResponse(Group group)
    {
        return new GroupResponse(
            group.Id,
            group.SpaceId,
            group.Name,
            group.Position,
            group.CreatedAt
        );
    }
}
