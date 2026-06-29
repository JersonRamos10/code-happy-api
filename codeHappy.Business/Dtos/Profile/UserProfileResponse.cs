namespace codeHappy.Business.Dtos.Profile
{
    public record UserProfileResponse(

        Guid Id,
        String UserName,
        string Email,
        string? AvatarUrl

    );
}