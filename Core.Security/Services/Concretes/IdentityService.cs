using System.Security.Claims;
using Core.Security.Services.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Core.Security.Services.Concretes;

public class IdentityService(IHttpContextAccessor httpContextAccessor) : IIdentityService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string GetUserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public string UserName => User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
