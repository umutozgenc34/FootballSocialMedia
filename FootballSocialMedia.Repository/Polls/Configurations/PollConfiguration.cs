using FootballSocialMedia.Model.Polls.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballSocialMedia.Repository.Polls.Configurations;

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.ToTable("Polls");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Question)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ExpiryDate)
            .IsRequired();

        builder.HasMany(x => x.Options)
            .WithOne(x => x.Poll)
            .HasForeignKey(x => x.PollId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
