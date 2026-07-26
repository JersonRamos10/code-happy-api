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
    public async Task<SnippetResponse> CreateSnippetAsync(Guid userId, CreateSnippetRequest req)
    {
        var profile = await _context.Profiles
                    .FindAsync(userId)
                    ?? throw new NotFoundException("Profile", userId);

        var snippet = new Snippet
        {
            Title = req.Title,
            OwnerId = profile.Id,
            SpaceId = req.SpaceId,
            GroupId = req.GroupId,
            Visibility = req.Visibility,
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

        await _context.Snippets.AddAsync(snippet);
        await _context.SaveChangesAsync();

        return MapToResponse(snippet);
    }

    // Deletes a snippet. Throws ForbiddenException if the user is not the owner.
    public async Task DeleteSnippetbyId(Guid userId, Guid snippetId)
    {
        var snippet = await _context.Snippets
            .Include(s => s.Blocks)
            .FirstOrDefaultAsync(s => s.Id == snippetId)
            ?? throw new NotFoundException("Snippet", snippetId);

        if (snippet.OwnerId != userId)
            throw new ForbiddenException();

        var imagePublicIds = snippet.Blocks
            .Where(b => b.ImageMetadata is not null)
            .Select(b => b.ImageMetadata!.PublicId)
            .ToList();

        _context.Remove(snippet);
        await _context.SaveChangesAsync();

        foreach (var publicId in imagePublicIds)
            await _imagesService.DestroyAssetBestEffortAsync(publicId, CancellationToken.None);
    }

    public async Task<PagedResponse<SnippetResponse>> GetAllSnippetAsync(Guid userId, SnippetParamsRequest req)
    {
        var profile = await _context.Profiles
                .FindAsync(userId)
                ?? throw new ForbiddenException();

        var query = _context.Snippets.AsNoTracking().AsQueryable();

        query = query.Where(s => s.OwnerId == userId || s.Visibility == SnippetVisibility.Public);

        if (req.GroupId.HasValue)
            query = query.Where(s => s.GroupId == req.GroupId);

        if (req.SpaceId.HasValue)
            query = query.Where(s => s.SpaceId == req.SpaceId);

        if (req.IsFavorite.HasValue)
            query = query.Where(s => s.IsFavorite == req.IsFavorite);

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.Title)
            .Skip((req.PageNumber - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / req.PageSize);

        return new PagedResponse<SnippetResponse>(
            items.Select(MapToResponse).ToList(),
            totalItems,
            totalPages,
            req.PageNumber,
            req.PageSize
        );
    }

    public async Task<SnippetResponse> GetSnippetByIdAsync(Guid userId, Guid snippetId)
    {
        var snippet = await _context.Snippets
            .FirstOrDefaultAsync(s => s.Id == snippetId)
            ?? throw new NotFoundException("Snippet", snippetId);

         if (snippet.OwnerId != userId && snippet.Visibility != SnippetVisibility.Public)
            throw new ForbiddenException();
        
        //Increment of property viewCount before to mapping
        snippet.ViewCount++;

        await _context.SaveChangesAsync();

        return MapToResponse(snippet);

    }

    // Destructive update: deletes all existing blocks and recreates them from the request.
    public async Task UpdateSnippetWithBlocksAsync(Guid snippetId, Guid userId, UpdateSnippetRequest req)
    {
        var snippet = await _context.Snippets
            .Include(s => s.Blocks)
            .FirstOrDefaultAsync(s => s.Id == snippetId)
            ?? throw new NotFoundException("Snippet", snippetId);

        if (snippet.OwnerId != userId)
            throw new ForbiddenException();

        snippet.Title = req.Title;
        snippet.Description = req.Description;
        snippet.Visibility = req.Visibility;
        snippet.GroupId = req.GroupId;
        snippet.SpaceId = req.SpaceId;
        snippet.UpdatedAt = DateTime.UtcNow;

        var incomingPublicIds = req.Blocks
            .Where(b => b.Type == BlockType.Image && b.PublicId is not null)
            .Select(b => b.PublicId!)
            .ToHashSet();

        var publicIdsToDestroy = snippet.Blocks
            .Where(b => b.ImageMetadata is not null && !incomingPublicIds.Contains(b.ImageMetadata.PublicId))
            .Select(b => b.ImageMetadata!.PublicId)
            .ToList();

        _context.Blocks.RemoveRange(snippet.Blocks);

        snippet.Blocks = req.Blocks.Select((b, index) => new Block
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
        }).ToList();

        await _context.SaveChangesAsync();

        foreach (var publicId in publicIdsToDestroy)
            await _imagesService.DestroyAssetBestEffortAsync(publicId, CancellationToken.None);
    }
    public async Task RecordCopy(Guid snippetId)
    {
        var snippet = await _context.Snippets
            .FindAsync(snippetId)
            ?? throw new NotFoundException("Snippet", snippetId);

        snippet.CopyCount++;

        await _context.SaveChangesAsync();
    }

    // Toggles the favorite state of a snippet.
    public async Task ToggleFavorite(Guid snippetId)
    {
        var snippet = await _context.Snippets
            .FindAsync(snippetId)
            ?? throw new NotFoundException("Snippet", snippetId);

        snippet.IsFavorite = !snippet.IsFavorite;

        await _context.SaveChangesAsync();
    }

    private static SnippetResponse MapToResponse(Snippet snippet)
    {
        return new SnippetResponse(
            snippet.Id,
            snippet.Title,
            snippet.Description,
            snippet.Visibility,
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