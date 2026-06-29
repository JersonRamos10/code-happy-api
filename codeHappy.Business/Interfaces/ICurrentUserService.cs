namespace codeHappy.Business.Interfaces;

public interface ICurrentUserService
{
    string? GetUserId();
    string? GetEmail();
    string? GetUserName();
    string? GetDisplayName();
}