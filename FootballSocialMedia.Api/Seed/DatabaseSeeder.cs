using Core.Security.Constants;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Api.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Bekleyen migration'ları otomatik uygula (Update-Database çalıştırmayı unutsan da DB güncel olur)
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { RoleNames.User, RoleNames.Admin })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // appsettings.Development.json → "SeedAdmin": { "UserName", "Email", "Password" }
        var adminSection = configuration.GetSection("SeedAdmin");
        var email = adminSection["Email"];
        var password = adminSection["Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new User { UserName = adminSection["UserName"] ?? "admin", Email = email };
            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                logger.LogWarning("Admin kullanıcısı oluşturulamadı: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }

            context.UserProfiles.Add(new UserProfile { Id = Guid.NewGuid(), UserId = admin.Id, Bio = "Site yöneticisi" });
            await context.SaveChangesAsync();
            logger.LogInformation("Admin kullanıcısı oluşturuldu: {Email}", email);
        }

        foreach (var role in new[] { RoleNames.User, RoleNames.Admin })
        {
            if (!await userManager.IsInRoleAsync(admin, role))
                await userManager.AddToRoleAsync(admin, role);
        }
    }
}
