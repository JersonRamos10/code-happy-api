using System.Security.Claims;
using codeHappy.Business.Interfaces;
using Microsoft.AspNetCore.Http;

namespace codeHappy.Api.Services;

public class CurrentUserService : ICurrentUserService
{

    private readonly IHttpContextAccessor _accesor;
    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accesor = accessor;
    }
    public string? GetDisplayName()
    {
        return _accesor.HttpContext?
                .User.FindFirstValue("display_name");

    }

    public string? GetEmail()
    {
        return _accesor.HttpContext?
            .User.FindFirstValue(ClaimTypes.Email);
    }

    public string? GetUserId()
    {
        return _accesor.HttpContext?
            .User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public string? GetUserName()
    {
        return _accesor.HttpContext?
            .User.FindFirstValue(ClaimTypes.Name);
    }
}