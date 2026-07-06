using codeHappy.Business.Dtos.Snippet;
using codeHappy.Business.Dtos.Pagined;

namespace codeHappy.Business.Interfaces;

public interface ISnippetService
{
    Task<PagedResponse<SnippetResponse>> GetAllSnippetAsync(Guid userId, SnippetParamsRequest req);

    Task<SnippetResponse> GetSnippetByIdAsync(Guid userId, Guid snippetId);

    Task<SnippetResponse> CreateSnippetAsync(Guid UserId, CreateSnippetRequest req);

    Task UpdateSnippetWithBlocksAsync(Guid snippetId, Guid userId, UpdateSnippetRequest req);

    Task ToggleFavorite(Guid snippetId);

    Task RecordCopy(Guid snippetId);

    Task DeleteSnippetbyId(Guid userId, Guid snippetId);

}