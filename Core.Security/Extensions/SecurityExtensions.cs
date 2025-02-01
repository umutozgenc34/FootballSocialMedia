using Core.Security.Services.Abstracts;
using Core.Security.Services.Concretes;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Security.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurityExtension(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
