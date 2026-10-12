using Core.Persistence.Entity;

namespace FootballSocialMedia.Model.Polls.Entities;

public class Poll : BaseEntity<Guid>, IAuditEntity
{
    public string Question { get; set; } = string.Empty;
    public List<PollOption> Options { get; set; } = new();
    public List<PollVote> Votes { get; set; } = new();
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
