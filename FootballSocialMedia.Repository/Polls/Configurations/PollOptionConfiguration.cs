using FootballSocialMedia.Model.Polls.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballSocialMedia.Repository.Polls.Configurations;

public class PollOptionConfiguration : IEntityTypeConfiguration<PollOption>
{
    public void Configure(EntityTypeBuilder<PollOption> builder)
    {
        builder.ToTable("PollOptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OptionText)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x =>  x.VoteCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.Poll)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.PollId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
