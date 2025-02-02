using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Likes.Entities;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Model.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FootballSocialMedia.Repository.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User,IdentityRole,string>(options)
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
