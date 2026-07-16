using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Interfaces
{
    public interface IBlocksService
    {
        Task<BlocksResponse> CreateBlockAsync(Guid userId, Guid snippetId, CreateBlockRequest request);
        Task UpdateBlockContentAsync(Guid userId, Guid snippetId, Guid blockId, UpdateBlockRequest request);

        Task UpdateBlockAnnotations(Guid UserId, Guid snippetId, Guid blockId, List<CreateAnnotationRequest>? request);

        Task DeleteBlock(Guid UserId, Guid snippetId, Guid blockId);

        Task ReorderBlocks(Guid UserId, Guid snippetId, List<ReorderBlockRequest> reorderBlocks);
    }
}
