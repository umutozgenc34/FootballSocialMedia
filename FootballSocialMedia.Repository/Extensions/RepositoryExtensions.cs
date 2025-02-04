using Core.Persistence.Options;
using FootballSocialMedia.Repository.ChatMessages.Abstracts;
using FootballSocialMedia.Repository.ChatMessages.Concretes;
using FootballSocialMedia.Repository.Comments.Abstracts;
using FootballSocialMedia.Repository.Comments.Concretes;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Follows.Abstracts;
using FootballSocialMedia.Repository.Follows.Concretes;
using FootballSocialMedia.Repository.Interceptors;
using FootballSocialMedia.Repository.Likes.Abstracts;
using FootballSocialMedia.Repository.Likes.Concretes;
using FootballSocialMedia.Repository.Newss.Abstracts;
using FootballSocialMedia.Repository.Newss.Concretes;
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
        services.Decorate<IPostRepository,PostRepositoryWithCache>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ILikeRepository,LikeRepository>();
        services.AddScoped<INewsRepository,NewsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserProfileRepository,UserProfileRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<IChatRepository,ChatRepository>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });


        return services;
    }
}
