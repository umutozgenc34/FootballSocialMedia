using Core.Infrastructure.Services.CloudinaryService.Abstracts;
using Core.Infrastructure.Services.CloudinaryService.Concretes;
using Core.Infrastructure.Services.CloudinaryService;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using Core.Infrastructure.Services.IdGenerator.Concretes;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Infrastructure.SignalR;
using Core.Infrastructure.RabbitMQ;

namespace Core.Infrastructure.Extensions;

public static class CommonServiceExtensions
{
    public static IServiceCollection AddCommonServiceExtension(this IServiceCollection services, Type assembly,IConfiguration configuration)
    {
        services.AddAutoMapper(assembly);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining(assembly);

        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

        services.AddScoped<IIdGenerator, IdGenerator>();
        services.AddSignalR();
        services.AddSingleton<RabbitMQClient>();
        return services;
    }
}
