using codeHappy.Business.Dtos.Blocks;
using codeHappy.Business.Dtos.Pagined;
using codeHappy.Business.Dtos.Snippet;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Business.Mappers;
using codeHappy.Data.Context;
using codeHappy.Data.Enums;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace codeHappy.Business.Services;

public class SnippetService(CodeHappyContext context, IImagesService imagesService) : ISnippetService
{
    private readonly CodeHappyContext _context = context;
    private readonly IImagesService _imagesService = imagesService;

    // Creates a snippet with its blocks. Verifies the user exists before creating.
    public async Task<SnippetResponse> CreateSnippetAsync(Guid userId, CreateSnippetRequest req, CancellationToken ct)
    {
        var profile = await _context.Profiles
                    .FindAsync([userId], ct)
                    ?? throw new NotFoundException("Profile", userId);

        var snippet = new Snippet
        {
            Title = req.Title,
            Description = req.Description,
            OwnerId = profile.Id,
            SpaceId = req.SpaceId,
            GroupId = req.GroupId,
            Visibility = req.Visibility,
            Topics = req.Topics ?? [],
            Blocks = req.Blocks.Select((b, index) => new Block
            {
                Content = b.Content,
                Title = b.Title,
                Type = b.Type,
                Language = b.Language,
                Position = index,
                ImageMetadata = b.Type != BlockType.Image ? null : new ImageMetadata
                {
                    PublicId = b.PublicId!,
                    SecureUrl = b.Content,
                    Width = b.Width,
                    Height = b.Height,
                    Format = b.Format,
                    Bytes = b.Bytes
                }
            }).ToList(),
        };

        await _context.Snippets.AddAsync(snippet, ct);
        await _context.SaveChangesAsync(ct);

        return MapToResponse(snippet);
    }

    // Deletes a snippet. Throws ForbiddenException if the user is not the owner.
    public async Task DeleteSnippetbyId(Guid userId, Guid snippetId, CancellationToken ct)
    {
        var snippet = await _context.Snippets
            .Include(s => s.Blocks)
            .FirstOrDefaultAsync(s => s.Id == snippetId, ct)
            ?? throw new NotFoundException("Snippet", snippetId);

        if (snippet.OwnerId != userId)
            throw new ForbiddenException();

        var imagePublicIds = snippet.Blocks
            .Where(b => b.ImageMetadata is not null)
            .Select(b => b.ImageMetadata!.PublicId)
            .ToList();

        _context.Remove(snippet);
        await _context.SaveChangesAsync(ct);

        foreach (var publicId in imagePublicIds)
            await _imagesService.DestroyAssetBestEffortAsync(publicId, ct);
    }

    public async Task MoveSnippetAsync(Guid userId, Guid snippetId, MoveSnippetRequest req, CancellationToken ct)
    {
        var snippet = await _context.Snippets
            .FindAsync([snippetId], ct)
            ?? throw new NotFoundException("Snippet", snippetId);

        if (snippet.OwnerId != userId)
            throw new ForbiddenException();

        if (req.SpaceId.HasValue)
        {
            var space = await _context.Spaces
                .FindAsync([req.SpaceId.Value], ct)
                ?? throw new NotFoundException("Space", req.SpaceId.Value);

            if (space.OwnerId != userId)
                throw new ForbiddenException();
        }

        if (req.GroupId.HasValue)
        {
            var group = await _context.Groups
                .FindAsync([req.GroupId.Value], ct)
                ?? throw new NotFoundException("Group", req.GroupId.Value);

            if (group.SpaceId != req.SpaceId!.Value)
                throw new NotFoundException("Group", req.GroupId.Value);
        }

        var targetSpaceId = req.SpaceId;
        var targetGroupId = req.GroupId;

        if (snippet.SpaceId == targetSpaceId && snippet.GroupId == targetGroupId)
            return;

        snippet.SpaceId = targetSpaceId;
        snippet.GroupId = targetGroupId;
        snippet.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<PagedResponse<SnippetSummaryResponse>> GetAllSnippetAsync(Guid userId, SnippetParamsRequest req, CancellationToken ct)
    {
        var profile = await _context.Profiles
                .FindAsync([userId], ct)
                ?? throw new ForbiddenException();

        var query = _context.Snippets.AsNoTracking().AsQueryable();

        query = query.Where(s => s.OwnerId == userId);

        if (req.GroupId.HasValue)
            query = query.Where(s => s.GroupId == req.GroupId);

        if (req.SpaceId.HasValue)
            query = query.Where(s => s.SpaceId == req.SpaceId);

        if (req.IsFavorite.HasValue)
            query = query.Where(s => s.IsFavorite == req.IsFavorite);

        var totalItems = await query.CountAsync(ct);

        var items = await query
            .OrderBy(s => s.Title)
            .Skip((req.PageNumber - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync(ct);

        int totalPages = (int)Math.Ceiling((double)totalItems / req.PageSize);

        return new PagedResponse<SnippetSummaryResponse>(
            items.Select(MapToSummaryResponse).ToList(),
            req.PageNumber,
            req.PageSize,
            totalItems,
            totalPages
        );
    }

    public async Task<SnippetResponse> GetSnippetByIdAsync(Guid userId, Guid snippetId, CancellationToken ct)
    {
        var snippet = await _context.Snippets
            .Include(s => s.Blocks.OrderBy(b => b.Position))
            .FirstOrDefaultAsync(s => s.Id == snippetId, ct)
            ?? throw new NotFoundException("Snippet", snippetId);

         if (snippet.OwnerId != userId && snippet.Visibility != SnippetVisibility.Public)
            throw new ForbiddenException();

        //Increment of property viewCount before to mapping
        snippet.ViewCount++;

        await _context.SaveChangesAsync(ct);

        return MapToResponse(snippet);

    }

    // Destructive update: deletes all existing blocks and recreates them from the request.
    public async Task UpdateSnippetWithBlocksAsync(Guid snippetId, Guid userId, UpdateSnippetRequest req, CancellationToken ct)
    {
        var snippet = await _context.Snippets
                          .Include(s => s.Blocks)
                          .Include(snippet => snippet.Shares)
                          .FirstOrDefaultAsync(s => s.Id == snippetId, ct)
            ?? throw new NotFoundException("Snippet", snippetId);



        if (snippet.OwnerId != userId)
            throw new ForbiddenException();

        snippet.Title = req.Title;
        snippet.Description = req.Description;
        snippet.Visibility = req.Visibility;
        snippet.GroupId = req.GroupId;
        snippet.SpaceId = req.SpaceId;
        snippet.Topics = req.Topics ?? [];
        snippet.UpdatedAt = DateTime.UtcNow;

        if (snippet.Visibility ==  SnippetVisibility.Private)
            _context.Shares.RemoveRange(snippet.Shares);

        var incomingPublicIds = req.Blocks
            .Where(b => b.Type == BlockType.Image && b.PublicId is not null)
            .Select(b => b.PublicId!)
            .ToHashSet();

        var publicIdsToDestroy = snippet.Blocks
            .Where(b => b.ImageMetadata is not null && !incomingPublicIds.Contains(b.ImageMetadata.PublicId))
            .Select(b => b.ImageMetadata!.PublicId)
            .ToList();

        _context.Blocks.RemoveRange(snippet.Blocks);

        var replacementBlocks = req.Blocks.Select((b, index) => new Block
        {
            SnippetId = snippet.Id,
            Content = b.Content,
            Title = b.Title,
            Type = b.Type,
            Language = b.Language,
            Position = index,
            ImageMetadata = b.Type != BlockType.Image ? null : new ImageMetadata
            {
                PublicId = b.PublicId!,
                SecureUrl = b.Content,
                Width = b.Width,
                Height = b.Height,
                Format = b.Format,
                Bytes = b.Bytes
            }
        }).ToList();

        await _context.Blocks.AddRangeAsync(replacementBlocks, ct);

        await _context.SaveChangesAsync(ct);

        foreach (var publicId in publicIdsToDestroy)
            await _imagesService.DestroyAssetBestEffortAsync(publicId, ct);
    }

    // Records a copy. Allowed for the owner and for any user on a public snippet,
    // mirroring the read access rule of GetSnippetByIdAsync.
    public async Task RecordCopy(Guid userId, Guid snippetId, CancellationToken ct)
    {
        var snippet = await _context.Snippets
            .FindAsync([snippetId], ct)
            ?? throw new NotFoundException("Snippet", snippetId);

        if (snippet.OwnerId != userId && snippet.Visibility != SnippetVisibility.Public)
            throw new ForbiddenException();

        snippet.CopyCount++;

        await _context.SaveChangesAsync(ct);
    }

    // Toggles the favorite state of a snippet. Restricted to the owner.
    public async Task ToggleFavorite(Guid userId, Guid snippetId, CancellationToken ct)
    {
        var snippet = await _context.Snippets
            .FindAsync([snippetId], ct)
            ?? throw new NotFoundException("Snippet", snippetId);

        if (snippet.OwnerId != userId)
            throw new ForbiddenException();

        snippet.IsFavorite = !snippet.IsFavorite;

        await _context.SaveChangesAsync(ct);
    }

    private static SnippetSummaryResponse MapToSummaryResponse(Snippet snippet)
    {
        return new SnippetSummaryResponse(
            snippet.Id,
            snippet.Title,
            snippet.Description,
            snippet.Visibility,
            snippet.IsFavorite,
            snippet.ViewCount,
            snippet.CopyCount,
            snippet.CreatedAt,
            snippet.UpdatedAt,
            snippet.Topics?.ToList(),
            snippet.SpaceId,
            snippet.GroupId
        );
    }

    private static SnippetResponse MapToResponse(Snippet snippet)
    {
        return new SnippetResponse(
            snippet.Id,
            snippet.Title,
            snippet.Description,
            snippet.Visibility,
            snippet.IsFavorite,
            snippet.ViewCount,
            snippet.CopyCount,
            snippet.CreatedAt,
            snippet.UpdatedAt,
            snippet.Topics?.ToList(),
            snippet.Blocks.Select(MapToBlockResponse).ToList(),
            snippet.SpaceId,
            snippet.GroupId
        );
    }

    private static BlocksResponse MapToBlockResponse(Block block)
    {
        return new BlocksResponse(
            block.Id,
            block.Title,
            block.Content,
            block.Language,
            block.Type,
            block.Annotations.Select(a => MapToAnnotationResponse(a)).ToList(),
            block.Position,
            ImageMetadata: block.ImageMetadata is null ? null : ImageMetadataMapper.ToResponse(block.ImageMetadata),
            block.CreatedAt,
            block.UpdatedAt
        );
    }

    private static AnnotationResponse MapToAnnotationResponse(CodeAnnotation annotation)
    {
        return new AnnotationResponse(
            Id: annotation.Id,
            LineNumber: annotation.LineNumber,
            Text: annotation.Text
        );
    }

}
