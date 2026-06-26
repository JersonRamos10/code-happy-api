namespace codeHappy.Business.Dtos
{
    public record UserProfileResponse(

        Guid Id,
        String UserName,
        string Email,
        string? AvatarUrl

    );
}