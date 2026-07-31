using codeHappy.Business.Dtos.Comments;

namespace codeHappy.Business.Interfaces;

public interface ICommentService
{
    Task<CommentResponse> CreateCommentAsync (
        Guid userId,
        CreateCommentRequest request, 
        Guid? shareId,
        Guid snippetId,
        CancellationToken ct);

    Task<IEnumerable<CommentResponse>> GetAllCommentsAsync(
        Guid userId,
        Guid snippetId,
        Guid? shareId,
        CancellationToken ct);

    Task UpdateCommentAsync(
        Guid userId, 
        Guid snippetId,
        Guid commentId, 
        Guid? shareId,
        UpdateCommentRequest request, 
        CancellationToken ct);

    Task DeleteCommentAsync(
        Guid userId,
        Guid commentId, 
        CancellationToken ct);
}