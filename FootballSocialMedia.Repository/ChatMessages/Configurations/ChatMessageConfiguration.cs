using FootballSocialMedia.Model.ChatMessages.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballSocialMedia.Repository.ChatMessages.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Timestamp)
           .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.Message)
            .IsRequired();

        builder.HasOne(x => x.Sender)
            .WithMany() 
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(x => x.Receiver)
            .WithMany()
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
       
    }
}

