

using codeHappy.Business.Dtos.Group;

namespace codeHappy.Business.Interfaces;

public interface IGroupService
{
    Task<GroupResponse> CreateGroupAsync(Guid spaceId, Guid userId, string name);
    Task<IEnumerable<GroupResponse>> GetAllGroupsAsync(Guid spaceId, Guid userId);

    Task UpdateGroupAsync(Guid groupId, Guid userId, string name);
    Task DeleteGroupAsync(Guid groupId, Guid userId);

}