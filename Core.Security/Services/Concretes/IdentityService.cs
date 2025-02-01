using System.Security.Claims;
using Core.Security.Services.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Core.Security.Services.Concretes;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                Console.WriteLine("IdentityService: UserId is null or empty.");
            }
            else
            {
                Console.WriteLine($"IdentityService: UserId = {userIdClaim}");
            }

            return userIdClaim ?? string.Empty;
        }
    }

    public string UserName
    {
        get
        {
            var userNameClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userNameClaim))
            {
                Console.WriteLine("IdentityService: UserName is null or empty.");
            }
            else
            {
                Console.WriteLine($"IdentityService: UserName = {userNameClaim}");
            }

            return userNameClaim ?? string.Empty;
        }
    }
}
