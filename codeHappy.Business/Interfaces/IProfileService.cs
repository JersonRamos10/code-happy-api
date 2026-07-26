using codeHappy.Business.Dtos.Profile;

namespace codeHappy.Business.Interfaces;

public interface IProfileService
{
    public Task<UserProfileResponse> GetUserbyIdAsync(Guid userId, CancellationToken ct);

    public Task SyncProfileAsync(Guid userId, string email, string userName, string displayName, CancellationToken ct);

}

