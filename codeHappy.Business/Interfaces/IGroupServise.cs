

using codeHappy.Business.Dtos.Groups;

namespace codeHappy.Business.Interfaces;

public interface IGroupService
{
    Task<GroupResponse> CreateGroupAsync(Guid spaceId, Guid userId, string name, CancellationToken ct);
    Task<IEnumerable<GroupResponse>> GetAllGroupsAsync(Guid spaceId, Guid userId, CancellationToken ct);
    Task<GroupResponse> GetGroupAsync(Guid spaceId, Guid groupId, Guid userId, CancellationToken ct);

    Task UpdateGroupAsync(Guid spaceId, Guid groupId, Guid userId, string name, CancellationToken ct);
    Task DeleteGroupAsync(Guid spaceId, Guid groupId, Guid userId, CancellationToken ct);

    Task ReorderGroupsAsync(Guid spaceId, Guid userId, List<ReorderGroupRequest> reorderGroups, CancellationToken ct);

}
