using FootballSocialMedia.Model.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballSocialMedia.Repository.Users.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.ToTable("Users");

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(256); 

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);


        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(15); 


        builder.HasMany(u => u.Posts) 
            .WithOne(p => p.User) 
            .HasForeignKey(p => p.UserId) 
            .OnDelete(DeleteBehavior.Cascade);
    }
}
