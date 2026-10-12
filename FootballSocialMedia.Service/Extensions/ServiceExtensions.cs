using Core.Security.Token.Encryption;
using Core.Security.Token.Options;
using FootballSocialMedia.Service.Authentications.Abstracts;
using FootballSocialMedia.Service.Authentications.Concretes;
using FootballSocialMedia.Service.ChatMessages.Abstracts;
using FootballSocialMedia.Service.ChatMessages.Concretes;
using FootballSocialMedia.Service.Comments.Abstracts;
using FootballSocialMedia.Service.Comments.Concretes;
using FootballSocialMedia.Service.Elasticsearch.Abstracts;
using FootballSocialMedia.Service.Elasticsearch.Concretes;
using FootballSocialMedia.Service.Follows.Abstracts;
using FootballSocialMedia.Service.Follows.Concretes;
using FootballSocialMedia.Service.JWT.Abstracts;
using FootballSocialMedia.Service.JWT.Concretes;
using FootballSocialMedia.Service.Likes.Abstracts;
using FootballSocialMedia.Service.Likes.Concretes;
using FootballSocialMedia.Service.Newss.Abstracts;
using FootballSocialMedia.Service.Newss.Concretes;
using FootballSocialMedia.Service.Notifications.Abstracts;
using FootballSocialMedia.Service.Notifications.Concretes;
using FootballSocialMedia.Service.Polls.Abstracts;
using FootballSocialMedia.Service.Polls.Concretes;
using FootballSocialMedia.Service.Posts.Abstracts;
using FootballSocialMedia.Service.Posts.Concretes;
using FootballSocialMedia.Service.RabbitMQ;
using FootballSocialMedia.Service.Roles.Abstracts;
using FootballSocialMedia.Service.Roles.Concretes;
using FootballSocialMedia.Service.Search.Abstracts;
using FootballSocialMedia.Service.Search.Concretes;
using FootballSocialMedia.Service.Users.Abstracts;
using FootballSocialMedia.Service.Users.Concretes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace FootballSocialMedia.Service.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtension(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRefreshTokenService, UserRefreshTokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ICommentService,CommentService>();
        services.AddScoped<ILikeService,LikeService>();
        services.AddScoped<INewsService,NewsService>();
        services.AddScoped<IFollowService, FollowService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPollService,PollService>();
        services.AddScoped<IElasticsearchService,ElasticsearchService>();
        services.AddScoped<ISearchService, SearchService>();

        services.AddSingleton<MessageArchiverProducer>();
        services.AddHostedService<MessageArchiverBackgroundService>();
        services.AddHostedService<MessageArchiverConsumer>();
        services.AddSingleton<INotificationPublisher, NotificationPublisher>();
        services.AddSingleton<NotificationConsumer>();
        services.AddHostedService<NotificationWorker>();

        services.Configure<CustomTokenOption>(configuration.GetSection("TokenOption"));

        // Not: Program.cs'te AddIdentity yerine AddIdentityCore kullanılıyor.
        // AddIdentity varsayılan şemayı cookie'ye çevirdiği için [Authorize] JWT yerine cookie arıyor ve 401/302 dönüyordu.
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
        {
            var tokenOptions = configuration.GetSection("TokenOption").Get<CustomTokenOption>()!;
            opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,
                ValidIssuer = tokenOptions.Issuer,
                ValidAudience = tokenOptions.Audience[0],
                IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            // SignalR WebSocket bağlantısında Authorization header gönderilemiyor,
            // token query string'den (?access_token=...) okunuyor.
            opts.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/chathub"))
                        context.Token = accessToken;
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
