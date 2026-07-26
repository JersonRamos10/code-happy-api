

using codeHappy.Business.Dtos.Groups;

namespace codeHappy.Business.Interfaces;

public interface IGroupService
{
    Task<GroupResponse> CreateGroupAsync(Guid spaceId, Guid userId, string name, CancellationToken ct);
    Task<IEnumerable<GroupResponse>> GetAllGroupsAsync(Guid spaceId, Guid userId, CancellationToken ct);

    Task UpdateGroupAsync(Guid groupId, Guid userId, string name, CancellationToken ct);
    Task DeleteGroupAsync(Guid groupId, Guid userId, CancellationToken ct);

}