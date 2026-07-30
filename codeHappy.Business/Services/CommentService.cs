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

        var profile = await _context.Profiles
            .FindAsync([userId], ct);

        return new CommentResponse(
            comment.Id,
            comment.SnippetId,
            comment.Text,
            comment.CreatedAt,
            comment.UpdatedAt,
            comment.OwnerId,
            Profile: new UserProfileResponse(
                profile!.Id,
                profile.UserName,
                profile.Email,
                profile.AvatarUrl
            )
        );

    }

    public Task<IEnumerable<CommentResponse>> GetAllCommentsAsync(Guid userId, Guid snippetId, Guid? shareId,
        CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCommentAsync(Guid userId, Guid commentId, UpdateCommentRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCommentAsync(Guid userId, Guid commentId, CancellationToken ct)
    {
        throw new NotImplementedException();
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

}