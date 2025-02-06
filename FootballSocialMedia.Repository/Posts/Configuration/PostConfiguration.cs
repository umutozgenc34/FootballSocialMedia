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

        builder.HasMany(p => p.Comments)  
               .WithOne(c => c.Post)    
               .HasForeignKey(c => c.PostId) 
               .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(p => p.Content)
               .HasDatabaseName("IX_Posts_Content");

        builder.HasIndex(p => p.UserId)
               .HasDatabaseName("IX_Posts_UserId");

        //builder.Navigation(x => x.User).AutoInclude();
        //builder.Navigation(x=>x.Comments).AutoInclude();
        //builder.Navigation(x=>x.Likes).AutoInclude();
    }
}
