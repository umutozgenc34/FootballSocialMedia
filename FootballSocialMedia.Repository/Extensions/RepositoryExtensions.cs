using Core.Persistence.Options;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace FootballSocialMedia.Repository.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoryExtension(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionStrings = configuration.GetSection
            (ConnectionStringOption.Key).Get<ConnectionStringOption>();

            options.UseSqlServer(connectionStrings!.SqlServer, sqlServerOptionsAction =>
            {
                sqlServerOptionsAction.MigrationsAssembly(typeof(RepositoryAssembly).Assembly.FullName);
            });

            options.AddInterceptors(new AuditDbContextInterceptor());
        });

        return services;
    }
}
