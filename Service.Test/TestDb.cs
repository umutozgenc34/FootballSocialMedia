using Core.Infrastructure.Services.IdGenerator.Concretes;
using Core.Security.Services.Abstracts;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Interceptors;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Service.Test;

/// <summary>Her test için izole bir InMemory veritabanı ve "giriş yapmış kullanıcı" taklidi.</summary>
public static class TestDb
{
    public static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditDbContextInterceptor())
            .Options);

    public static User AddUser(AppDbContext context, string userName)
    {
        var user = new User { Id = Guid.NewGuid().ToString(), UserName = userName, Email = $"{userName}@test.com" };
        context.Users.Add(user);
        context.SaveChanges();
        return user;
    }

    /// <summary>GetUserId/UserName değerlerini test sırasında değiştirebilmek için mutable bir identity mock'u.</summary>
    public sealed class FakeIdentity
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public Mock<IIdentityService> Mock { get; } = new();

        public FakeIdentity()
        {
            Mock.SetupGet(x => x.GetUserId).Returns(() => UserId);
            Mock.SetupGet(x => x.UserName).Returns(() => UserName);
            Mock.SetupGet(x => x.IsAuthenticated).Returns(() => !string.IsNullOrEmpty(UserId));
            Mock.Setup(x => x.IsInRole(It.IsAny<string>())).Returns((string role) => role == "admin" && IsAdmin);
        }

        public void LoginAs(User user)
        {
            UserId = user.Id;
            UserName = user.UserName!;
        }

        public IIdentityService Object => Mock.Object;
    }

    public static IdGenerator IdGenerator { get; } = new();
}
