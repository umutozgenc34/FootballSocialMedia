using FootballSocialMedia.Model.News.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballSocialMedia.Repository.Newss.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("News");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200); 

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(5000); 

        builder.Property(n => n.Source)
            .IsRequired()
            .HasMaxLength(255); 

        builder.Property(n => n.ImageUrl)
            .HasMaxLength(500);
    }
}
