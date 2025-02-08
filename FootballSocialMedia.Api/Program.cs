using Core.Infrastructure.Extensions;
using Core.Infrastructure.Versioning;
using Core.Security.Extensions;
using FootballSocialMedia.Api.ExceptionHandler;
using FootballSocialMedia.Model.ChatMessages.Entities;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Extensions;
using FootballSocialMedia.Service;
using FootballSocialMedia.Service.ChatHubs;
using FootballSocialMedia.Service.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositoryExtension(builder.Configuration);
builder.Services.AddServiceExtension(builder.Configuration);
builder.Services.AddCommonServiceExtension(typeof(ServiceAssembly),builder.Configuration);
builder.Services.AddSecurityExtension();
builder.Services.AddVersioningExtension();
builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});


var app = builder.Build();

var apiVersionSet = app.AddVersionSetExtension();

app.UseRouting();

app.UseCors("AllowAll");
app.UseExceptionHandler(x => { });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub");
});

app.MapControllers();

app.Run();
