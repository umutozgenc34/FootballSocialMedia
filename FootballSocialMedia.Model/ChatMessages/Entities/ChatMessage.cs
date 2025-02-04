using Core.Persistence.Entity;
using FootballSocialMedia.Model.Users.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballSocialMedia.Model.ChatMessages.Entities;

public class ChatMessage : BaseEntity<Guid>
{
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public virtual User Sender { get; set; } = default!;
    public virtual User Receiver { get; set; } = default!;
}
