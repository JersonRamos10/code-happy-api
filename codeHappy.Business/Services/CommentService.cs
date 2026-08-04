using codeHappy.Business.Dtos.Comments;
using codeHappy.Business.Dtos.Profile;
using codeHappy.Business.Exceptions;
using codeHappy.Business.Interfaces;
using codeHappy.Data.Context;
using codeHappy.Data.Enums;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace codeHappy.Business.Services;

public class CommentService(CodeHappyContext context) : ICommentService
{

    private readonly CodeHappyContext _context = context;

    public async Task<CommentResponse> CreateCommentAsync(Guid userId, CreateCommentRequest request, Guid? shareId,
        Guid snippetId, CancellationToken ct)
    {
        var snippet = await GetAuthorizedSnippetAsync(userId, snippetId, shareId, ct);
        
        var comment = new Comment
        {
            OwnerId = userId,
            SnippetId = snippetId,
            Text = request.Text.Trim(),
            Snippet = snippet
        };

        await _context.Comments.AddAsync(comment, ct);
        await _context.SaveChangesAsync(ct);

        var profile = await _context.Profiles.FindAsync([userId], ct)
                      ?? throw new NotFoundException("profile", userId);
        
        comment.Profile = profile;
        return MapToCommentResponse(comment);

    }

    public async Task<IEnumerable<CommentResponse>> GetAllCommentsAsync(Guid userId, Guid snippetId, Guid? shareId,
        CancellationToken ct)
    {
        var snippet = await GetAuthorizedSnippetAsync(userId, snippetId, shareId, ct);
        
        var commentsOfSnippet = await _context.Comments
            .Where( c => c.SnippetId == snippetId)
            .AsNoTracking()
            .OrderBy(c => c.CreatedAt)
            .Select (c => new CommentResponse(
                c.Id,
                c.SnippetId,
                c.Text,
                c.CreatedAt,
                c.UpdatedAt,
                c.OwnerId,
                new UserProfileResponse(
                    c.Profile.Id,
                    c.Profile.UserName,
                    c.Profile.DisplayName,
                    c.Profile.Email,
                    c.Profile.AvatarUrl) 
                ))
            .ToListAsync(ct);

        return commentsOfSnippet;

    }

    public async Task UpdateCommentAsync(Guid userId, Guid snippetId,Guid commentId,Guid? shareId, UpdateCommentRequest request, CancellationToken ct)
    {
        var snippet = await GetAuthorizedSnippetAsync(userId, snippetId, shareId, ct);
        
        var comment = await 
            _context.Comments.
                FirstOrDefaultAsync( c => c.Id == commentId && c.SnippetId == snippetId,ct)
        ?? throw new NotFoundException("comment", commentId);

        if (comment.OwnerId != userId)
            throw new ForbiddenException();
        
        comment.Text = request.Text.Trim();
        comment.UpdatedAt = DateTime.UtcNow;
            
            
        await _context.SaveChangesAsync(ct);
        
    }

    public async Task DeleteCommentAsync(Guid userId, Guid commentId, CancellationToken ct)
    {
        var c = await _context.Comments.Include(c => c.Snippet)
                    .FirstOrDefaultAsync(c => c.Id == commentId,ct)
                ?? throw new NotFoundException("comment", commentId);

        if (c.Snippet.OwnerId != userId && c.OwnerId != userId)
            throw new ForbiddenException();

        _context.Comments.Remove(c);
        await _context.SaveChangesAsync(ct);
        
    }

    private async Task<Snippet> GetAuthorizedSnippetAsync(
        Guid userId,
        Guid snippetId,
        Guid? shareId,
        CancellationToken ct)
    {
        var snippet = await _context.Snippets.FirstOrDefaultAsync(s => s.Id == snippetId,ct)
                      ?? throw new NotFoundException("snippet", snippetId);


        if (snippet.OwnerId == userId || snippet.Visibility == SnippetVisibility.Public)
            return snippet;
        
        if (shareId is null)
            throw new ForbiddenException();
        
        var share = await _context.Shares
                        .FirstOrDefaultAsync(s => s.Id == shareId && s.SnippetId == snippetId, ct)
                    ?? throw new NotFoundException("share", shareId.Value);

        if (share.ExpiresAt.HasValue && share.ExpiresAt < DateTime.UtcNow)
            throw new NotFoundException("share", shareId.Value);

        return snippet;
    }


    private static CommentResponse MapToCommentResponse(Comment comment)
    {
        return new CommentResponse(
            comment.Id,
            comment.SnippetId,
            comment.Text,
            comment.CreatedAt,
            comment.UpdatedAt,
            comment.OwnerId,
            Profile: MapToUserProfileResponse(comment.Profile)
        );

    }
    private static UserProfileResponse MapToUserProfileResponse(Profile profile)
    {
        return new UserProfileResponse(
            profile!.Id,
            profile.UserName,
            profile.DisplayName,
            profile.Email,
            profile.AvatarUrl
        );
    }
}