using Core.Infrastructure.Extensions;
using Core.Infrastructure.Versioning;
using Core.Security.Extensions;
using FootballSocialMedia.Api.ExceptionHandler;
using FootballSocialMedia.Api.Seed;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Extensions;
using FootballSocialMedia.Service;
using FootballSocialMedia.Service.ChatHubs;
using FootballSocialMedia.Service.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositoryExtension(builder.Configuration);
builder.Services.AddServiceExtension(builder.Configuration);
builder.Services.AddCommonServiceExtension(typeof(ServiceAssembly), builder.Configuration);
builder.Services.AddSecurityExtension();
builder.Services.AddVersioningExtension();
builder.Services.AddHttpContextAccessor();

// AddIdentity yerine AddIdentityCore: AddIdentity varsayılan authentication şemasını cookie'ye çeviriyordu,
// bu yüzden JWT ile gelen istekler kimliksiz kalıyordu. API sadece JWT kullandığı için Core yeterli.
builder.Services.AddIdentityCore<User>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    // Enum'lar (ör. NotificationType) JSON'da sayı yerine "Like", "Follow" gibi string dönsün
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Validation hataları da diğer hatalarla aynı formatta ({ errorMessage: [...] }) dönsün
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Invalid request." : e.ErrorMessage)
            .ToList();
        return new BadRequestObjectResult(new { errorMessage = errors });
    };
});

builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FootballSocialMedia API", Version = "v1" });

    // Swagger'da "Authorize" butonu ile JWT girebilmek için
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Login'den dönen accessToken'ı yapıştırın."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS: AllowAnyOrigin() + SignalR birlikte çalışmaz (SignalR credentials gönderir, tarayıcı "*"ı reddeder).
// Front'un adresi açıkça yazılmalı ve AllowCredentials() eklenmeli.
var allowedOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                     ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

app.AddVersionSetExtension();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Development'ta https yönlendirmesi kapalı: front http://localhost:5187'ye istek atınca
    // CORS preflight (OPTIONS) isteği https'e redirect ediliyor ve tarayıcı bunu reddediyordu.
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

// Migration'ları uygula, rolleri ve (development'ta) admin kullanıcısını oluştur
await DatabaseSeeder.SeedAsync(app.Services, app.Configuration, app.Logger);

app.Run();

// WebApplicationFactory ile entegrasyon testleri yazılabilsin diye
public partial class Program;
