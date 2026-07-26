using codeHappy.Business.Dtos.Snippet;
using codeHappy.Business.Dtos.Pagined;

namespace codeHappy.Business.Interfaces;

public interface ISnippetService
{
    Task<PagedResponse<SnippetResponse>> GetAllSnippetAsync(Guid userId, SnippetParamsRequest req, CancellationToken ct);

    Task<SnippetResponse> GetSnippetByIdAsync(Guid userId, Guid snippetId, CancellationToken ct);

    Task<SnippetResponse> CreateSnippetAsync(Guid UserId, CreateSnippetRequest req, CancellationToken ct);

    Task UpdateSnippetWithBlocksAsync(Guid snippetId, Guid userId, UpdateSnippetRequest req, CancellationToken ct);

    Task ToggleFavorite(Guid snippetId, CancellationToken ct);

    Task RecordCopy(Guid snippetId, CancellationToken ct);

    Task DeleteSnippetbyId(Guid userId, Guid snippetId, CancellationToken ct);

}
