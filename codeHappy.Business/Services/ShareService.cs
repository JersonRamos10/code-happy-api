using codeHappy.Business.Dtos.Blocks;
using codeHappy.Business.Dtos.Shares;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Business.Mappers;
using codeHappy.Data.Context;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace codeHappy.Business.Services;

public class ShareService(CodeHappyContext context) : IShareService
{
    private readonly CodeHappyContext _context = context;

    public async Task<ShareResponse> CreateShareAsync(Guid userId, CreateShareRequest request)
    {
        var snippet = 
            await _context.Snippets
                .FirstOrDefaultAsync(s => s.Id == request.SnippetId) 
            ?? throw new NotFoundException("snippet", request.SnippetId);
        
        if(snippet.OwnerId != userId)
            throw new ForbiddenException();
        
        //Create share for snippet and date expiration 

        var share = new Share
        {
            SnippetId =  snippet.Id,
            SharedBy =  userId,
            ExpiresAt = request.ExpiresAt,
        };
        
        await _context.Shares.AddAsync(share);
        await _context.SaveChangesAsync();
        
        return MapToShareResponse(share);
        
    }

    public async Task<IEnumerable<ShareResponse>> GetMySharesAsync(Guid userId)
    {
        var shares = _context.Shares
            .AsNoTracking()
            .Where(s => s.SharedBy == userId)
            .Select(s => MapToShareResponse(s));

        return await shares.ToListAsync();
    }

    public async Task DeleteShareAsync(Guid userId, Guid shareId)
    {
        var share = await _context.Shares.FirstOrDefaultAsync(s => s.Id == shareId)
            ?? throw new NotFoundException("share", shareId);
        
        if(share.SharedBy !=  userId)
            throw new ForbiddenException();
        
        _context.Shares.Remove(share);
        await _context.SaveChangesAsync();
    }

    public async Task<SharedSnippetResponse> GetSharedSnippetAsync(Guid shareId)
    {
        var share = await _context.Shares
            .Include(s => s.Snippet)
                .ThenInclude(sn => sn.Blocks)
            .FirstOrDefaultAsync(s => s.Id == shareId)
            ?? throw new NotFoundException("share", shareId);

        if (share.ExpiresAt.HasValue && share.ExpiresAt < DateTime.UtcNow)
            throw new NotFoundException("share", shareId);

        return MapToSharedSnippetResponse(share.Snippet);
    }

    private static ShareResponse MapToShareResponse(Share share)
    {
        return new ShareResponse(
            Id: share.Id,
            SnippetId: share.SnippetId,
            ExpiresAt: share.ExpiresAt,
            CreatedAt: share.CreatedAt
            );
    }

    private static SharedSnippetResponse MapToSharedSnippetResponse(Snippet snippet)
    {
        return new SharedSnippetResponse(
            snippet.Id,
            snippet.Title,
            snippet.Description,
            snippet.Visibility,
            snippet.Topics?.ToList(),
            snippet.Blocks.Select(MapToBlockResponse).ToList()
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
            block.Annotations.Select(MapToAnnotationResponse).ToList(),
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