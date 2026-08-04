using System.Security.Claims;
using codeHappy.Business.Interfaces;
using Microsoft.AspNetCore.Http;

namespace codeHappy.Api.Services;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{

    private readonly IHttpContextAccessor _accesor = accessor;


    public string? GetEmail()
    {
        var user = _accesor.HttpContext?.User;

        return user?.FindFirstValue("email")
               ?? user?.FindFirstValue(ClaimTypes.Email);
    }

    public string? GetUserId()
    {
        var user = _accesor.HttpContext?.User;

        return user?.FindFirstValue("sub")
               ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

}