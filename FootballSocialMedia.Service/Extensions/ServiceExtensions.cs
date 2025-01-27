using FootballSocialMedia.Service.Posts.Abstracts;
using FootballSocialMedia.Service.Posts.Concretes;
using Microsoft.Extensions.DependencyInjection;

namespace FootballSocialMedia.Service.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtension(this IServiceCollection services)
    {

        services.AddScoped<IPostService, PostService>();

        return services;
    }
}
