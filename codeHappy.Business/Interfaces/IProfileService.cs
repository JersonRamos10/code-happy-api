using codeHappy.Business.Dtos;

namespace codeHappy.Business.Interfaces;

public interface IProfileService
{
    public Task<UserProfileResponse> GetUserbyIdAsync(Guid userId);

    public Task SyncProfileAsync(Guid userId, string email, string userName, string displayName);

}

