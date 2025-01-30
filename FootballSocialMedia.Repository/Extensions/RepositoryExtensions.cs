using Core.Persistence.Options;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Interceptors;
using FootballSocialMedia.Repository.Posts.Abstracts;
using FootballSocialMedia.Repository.Posts.Concretes;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Concretes;
using FootballSocialMedia.Repository.Users.Abstracts;
using FootballSocialMedia.Repository.Users.Concretes;
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


        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserProfileRepository,UserProfileRepository>();

        return services;
    }
}
