using codeHappy.Business.Dtos.Spaces;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace codeHappy.Business.Services;

public class SpaceService : ISpaceService
{
    private readonly CodeHappyContext _context;

    public SpaceService(CodeHappyContext context)
    {
        _context = context;
    }
    // Creates a new space for the authenticated user. Returns the created space.
    public async Task<SpaceResponse> CreateSpaceAsync(Guid userId, CreateSpaceRequest request)
    {
        var space = new Space
        {
            OwnerId = userId,
            Name = request.Name,
            Icon = request.Icon,
        };

        await _context.Spaces.AddAsync(space);
        await _context.SaveChangesAsync();

        return MapToResponse(space);
    }

    // Returns all spaces owned by the user.
    public async Task<IEnumerable<SpaceResponse>> GetAllSpacesAsync(Guid userId)
    {
        return await _context.Spaces
            .AsNoTracking()
            .Where(s => s.OwnerId == userId)
            .Select(s => MapToResponse(s))
            .ToListAsync();
    }

    // Updates name and icon. Throws ForbiddenException if the user is not the owner.
    public async Task UpdateSpaceAsync(Guid spaceId, Guid userId, UpdateSpaceRequest request)
    {
        var space = await _context.Spaces
                .FirstOrDefaultAsync(s => s.Id == spaceId) ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        space.Name = request.Name;
        space.Icon = request.Icon;
        space.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    // Updates LastAccessedAt to now. Used to track recently opened spaces.
    public async Task TouchSpaceAsync(Guid spaceId, Guid userId)
    {
        var space = await _context.Spaces
             .FirstOrDefaultAsync(s => s.Id == spaceId) ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        space.LastAccessedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // Deletes the space and cascades to groups. Snippets keep space_id as null.
    public async Task DeleteSpaceAsync(Guid spaceId, Guid userId)
    {
        var space = await _context.Spaces
            .FirstOrDefaultAsync(s => s.Id == spaceId) ?? throw new NotFoundException("Space", spaceId);

        if (space.OwnerId != userId)
            throw new ForbiddenException();

        _context.Remove(space);
        await _context.SaveChangesAsync();
    }


    private static SpaceResponse MapToResponse(Space space)
    {
        return new SpaceResponse(
            space.Id,
            space.Name,
            space.Icon,
            space.CreatedAt,
            space.UpdatedAt,
            space.LastAccessedAt
        );

    }
}