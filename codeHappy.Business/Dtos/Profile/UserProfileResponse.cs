namespace codeHappy.Business.Dtos.Profile
{
    public record UserProfileResponse(

        Guid Id,
        string UserName,
        string DisplayName,
        string Email,
        string? AvatarUrl

    );
}