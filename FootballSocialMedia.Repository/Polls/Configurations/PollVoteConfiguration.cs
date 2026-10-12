using FootballSocialMedia.Model.Polls.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballSocialMedia.Repository.Polls.Configurations;

public class PollVoteConfiguration : IEntityTypeConfiguration<PollVote>
{
    public void Configure(EntityTypeBuilder<PollVote> builder)
    {
        builder.ToTable("PollVotes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);

        // Kişi başı bir ankette tek oy
        builder.HasIndex(x => new { x.PollId, x.UserId }).IsUnique();

        builder.HasOne(x => x.Poll)
            .WithMany(x => x.Votes)
            .HasForeignKey(x => x.PollId)
            .OnDelete(DeleteBehavior.Cascade);

        // SQL Server multiple cascade path hatası vermemesi için option ve user tarafı NoAction
        builder.HasOne(x => x.PollOption)
            .WithMany()
            .HasForeignKey(x => x.PollOptionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
