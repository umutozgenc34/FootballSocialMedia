using Core.Infrastructure.Extensions;
using FootballSocialMedia.Repository.Extensions;
using FootballSocialMedia.Service;
using FootballSocialMedia.Service.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRepositoryExtension(builder.Configuration);
builder.Services.AddServiceExtension();
builder.Services.AddCommonServiceExtension(typeof(ServiceAssembly),builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
