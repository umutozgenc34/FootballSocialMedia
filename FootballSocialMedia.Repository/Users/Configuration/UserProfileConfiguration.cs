using FootballSocialMedia.Model.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballSocialMedia.Repository.Users.Configuration;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.UserId)
               .IsRequired()
               .HasMaxLength(450); 

        builder.Property(up => up.FavoriteTeam)
               .HasMaxLength(100);

        builder.Property(up => up.Bio)
               .HasMaxLength(500);

        builder.Property(up => up.ProfileImageUrl)
               .HasMaxLength(250);
        
        builder.HasOne(up => up.User) 
               .WithOne(u => u.Profile)  
               .HasForeignKey<UserProfile>(up => up.UserId)  
               .OnDelete(DeleteBehavior.Cascade);
    }
}
