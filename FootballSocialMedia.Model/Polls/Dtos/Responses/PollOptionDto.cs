namespace FootballSocialMedia.Model.Polls.Dtos.Responses;

public sealed record PollOptionDto
{
    public Guid Id { get; init; }
    public string OptionText { get; init; }
    public int VoteCount { get; init; }
}
