using Core.Infrastructure.Extensions;
using FootballSocialMedia.Api.ExceptionHandler;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Extensions;
using FootballSocialMedia.Service;
using FootballSocialMedia.Service.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRepositoryExtension(builder.Configuration);
builder.Services.AddServiceExtension();
builder.Services.AddCommonServiceExtension(typeof(ServiceAssembly),builder.Configuration);

builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler(x => { });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
