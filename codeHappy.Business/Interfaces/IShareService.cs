using codeHappy.Business.Dtos.Shares;

namespace codeHappy.Business.Interfaces;

public interface IShareService
{
   Task<ShareResponse> CreateShareAsync(Guid userId, CreateShareRequest request, CancellationToken ct);

   Task<IEnumerable<ShareResponse>> GetMySharesAsync(Guid userId, CancellationToken ct);

   Task DeleteShareAsync(Guid userId, Guid shareId, CancellationToken ct);

   Task<SharedSnippetResponse>  GetSharedSnippetAsync(Guid shareId, CancellationToken ct);
}
