using codeHappy.Business.Dtos.Shares;

namespace codeHappy.Business.Interfaces;

public interface IShareService
{
   Task<ShareResponse> CreateShareAsync(Guid userId, CreateShareRequest request);
   
   Task<IEnumerable<ShareResponse>> GetMySharesAsync(Guid userId);
   
   Task DeleteShareAsync(Guid userId, Guid shareId);
   
   Task<SharedSnippetResponse>  GetSharedSnippetAsync(Guid shareId);
}