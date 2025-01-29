using Microsoft.Extensions.DependencyInjection;

namespace Core.Security.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurityExtension(this IServiceCollection services)
    {
        return services;
    }
}
