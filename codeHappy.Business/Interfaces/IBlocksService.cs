using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Interfaces
{
    public interface IBlocksService
    {
        Task<BlocksResponse> CreateBlockAsync(Guid userId, Guid snippetId, CreateBlockRequest request, CancellationToken ct);
        Task UpdateBlockContentAsync(Guid userId, Guid snippetId, Guid blockId, UpdateBlockRequest request, CancellationToken ct);

        Task UpdateBlockAnnotations(Guid UserId, Guid snippetId, Guid blockId, List<CreateAnnotationRequest>? request, CancellationToken ct);

        Task DeleteBlock(Guid UserId, Guid snippetId, Guid blockId, CancellationToken ct);

        Task ReorderBlocks(Guid UserId, Guid snippetId, List<ReorderBlockRequest> reorderBlocks, CancellationToken ct);
    }
}
