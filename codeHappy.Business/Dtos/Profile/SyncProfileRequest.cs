namespace codeHappy.Business.Dtos.Profile;

public record SyncProfileRequest(
    string UserName,
    string DisplayName
    );