using codeHappy.Business.Dtos.Spaces;

namespace codeHappy.Business.Interfaces;

public interface ISpaceService
{

    Task<SpaceResponse> CreateSpaceAsync(Guid userId, CreateSpaceRequest request, CancellationToken ct);
    Task<IEnumerable<SpaceResponse>> GetAllSpacesAsync(Guid userId, CancellationToken ct);
    Task UpdateSpaceAsync(Guid spaceId, Guid userId, UpdateSpaceRequest request, CancellationToken ct);
    Task TouchSpaceAsync(Guid spaceId, Guid userId, CancellationToken ct);
    Task DeleteSpaceAsync(Guid spaceId, Guid userId, CancellationToken ct);
}
