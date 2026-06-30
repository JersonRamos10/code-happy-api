
using Microsoft.EntityFrameworkCore;
using codeHappy.Business.Dtos.Group;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Models;


namespace codeHappy.Business.Services;

public class GroupService : IGroupService
{

    private readonly CodeHappyContext _context;
    public GroupService(CodeHappyContext context)
    {
        _context = context;
    }


    // Creates a group inside a space. Position is calculated as the current group count in that space.
    public async Task<GroupResponse> CreateGroupAsync(Guid spaceId, Guid userId, string name)
    {
        var space = await _context.Spaces
                .Include(s => s.Groups)
                .FirstOrDefaultAsync(s => s.Id == spaceId)
                ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        var group = new Group
        {
            Name = name,
            SpaceId = spaceId,
            Position = space.Groups.Count,
        };

        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();

        return MapToResponse(group);
    }

    // Returns all groups in a space ordered by position ascending.
    public async Task<IEnumerable<GroupResponse>> GetAllGroupsAsync(Guid spaceId, Guid userId)
    {
        var space = await _context.Spaces
            .FirstOrDefaultAsync(s => s.Id == spaceId)
            ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        return await _context.Groups
                .Where(g => g.SpaceId == spaceId)
                .OrderBy(g => g.Position)
                .Select(g => MapToResponse(g))
                .ToListAsync();
    }

    // Renames the group. Verifies ownership through the parent space.
    public async Task UpdateGroupAsync(Guid groupId, Guid userId, string name)
    {
        var group = await _context.Groups
            .Include(g => g.Space)
            .FirstOrDefaultAsync(g => g.Id == groupId)
            ?? throw new NotFoundException("Group", groupId);

        if (group.Space.OwnerId != userId)
            throw new ForbiddenException();

        group.Name = name;
        group.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    // Deletes the group. Verifies ownership through the parent space.
    public async Task DeleteGroupAsync(Guid groupId, Guid userId)
    {
        var group = await _context.Groups
            .Include(g => g.Space)
            .FirstOrDefaultAsync(g => g.Id == groupId)
            ?? throw new NotFoundException("Group", groupId);

        if (group.Space.OwnerId != userId)
            throw new ForbiddenException();

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
    }

    private static GroupResponse MapToResponse(Group group)
    {
        return new GroupResponse(
            group.Id,
            group.Name,
            group.Position,
            group.CreatedAt
        );
    }
}