namespace codeHappy.Api.Dtos
{
    public record UserProfileResponse(

        Guid Id,
        String UserName,
        string Email,
        string? AvatarUrl

    );
}