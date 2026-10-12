using Core.Persistence.Entity;
using FootballSocialMedia.Model.Users.Entities;

namespace FootballSocialMedia.Model.Polls.Entities;

/// <summary>Bir kullanıcının bir ankette verdiği oy. (PollId, UserId) tekil → kişi başı tek oy.</summary>
public class PollVote : BaseEntity<Guid>, IAuditEntity
{
    public Guid PollId { get; set; }
    public Poll Poll { get; set; } = default!;
    public Guid PollOptionId { get; set; }
    public PollOption PollOption { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
