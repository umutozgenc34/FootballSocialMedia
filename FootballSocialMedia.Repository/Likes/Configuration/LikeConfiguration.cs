using FootballSocialMedia.Model.Likes.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FootballSocialMedia.Repository.Likes.Configuration;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.ToTable("Likes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
               .IsRequired()
               .HasMaxLength(450);

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Post)
               .WithMany(p => p.Likes)
               .HasForeignKey(l => l.PostId)
               .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(x => x.Comment)
               .WithMany(c => c.Likes)
               .HasForeignKey(l => l.CommentId)
               .OnDelete(DeleteBehavior.ClientSetNull);
    }
}