using FootballSocialMedia.Model.Posts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballSocialMedia.Repository.Posts.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(x=> x.Id);

        builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.ImageUrls)
            .HasMaxLength(2500);

        builder.Property(p => p.UserId)
           .IsRequired()
           .HasMaxLength(450);

        builder.HasOne(p => p.User)  
              .WithMany(u => u.Posts) 
              .HasForeignKey(p => p.UserId)  
              .OnDelete(DeleteBehavior.Cascade);  

        
    }
}
