using FootballSocialMedia.Model.Follows.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballSocialMedia.Repository.Follows.Configurations;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.HasKey(f => new { f.FollowerId, f.FollowingId }); // composite pk

        builder.Property(f => f.FollowerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(f => f.FollowingId)
            .IsRequired()
            .HasMaxLength(450); 

        builder.HasOne(f => f.Follower)
            .WithMany() 
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.NoAction); 

        builder.HasOne(f => f.Following)
            .WithMany() 
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
