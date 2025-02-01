using Core.Security.Token.Encryption;
using Core.Security.Token.Options;
using FootballSocialMedia.Service.Authentications.Abstracts;
using FootballSocialMedia.Service.Authentications.Concretes;
using FootballSocialMedia.Service.Comments.Abstracts;
using FootballSocialMedia.Service.Comments.Concretes;
using FootballSocialMedia.Service.JWT.Abstracts;
using FootballSocialMedia.Service.JWT.Concretes;
using FootballSocialMedia.Service.Posts.Abstracts;
using FootballSocialMedia.Service.Posts.Concretes;
using FootballSocialMedia.Service.Roles.Abstracts;
using FootballSocialMedia.Service.Roles.Concretes;
using FootballSocialMedia.Service.Users.Abstracts;
using FootballSocialMedia.Service.Users.Concretes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.Configure<CustomTokenOption>(configuration.GetSection("TokenOption"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
        {
        var tokenOptions = configuration.GetSection("TokenOption").Get<CustomTokenOption>();
          opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
         {
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience[0],
        IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
        };
        });

        return services;
    }
}
