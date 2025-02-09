using Core.Persistence.Entity;

namespace FootballSocialMedia.Model.Polls.Entities;

public class PollOption :  BaseEntity<Guid>
{
    public string OptionText { get; set; } = string.Empty;
    public int VoteCount { get; set; } = 0;
    public Guid PollId { get; set; }
    public Poll Poll { get; set; } = default!;
}
