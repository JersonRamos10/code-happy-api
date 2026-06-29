using codeHappy.Business.Dtos.Spaces;

namespace codeHappy.Business.Interfaces;

public interface ISpaceService
{

    Task<SpaceResponse> CreateSpaceAsync(Guid userId, CreateSpaceRequest request);
    Task<IEnumerable<SpaceResponse>> GetAllSpacesAsync(Guid userId);
    Task UpdateSpaceAsync(Guid spaceId, Guid userId, UpdateSpaceRequest request);
    Task TouchSpaceAsync(Guid spaceId, Guid userId);
    Task DeleteSpaceAsync(Guid spaceId, Guid userId);
}