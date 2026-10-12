namespace FootballSocialMedia.Model.Polls.Dtos.Responses;

public sealed record PollOptionDto
{
    public Guid Id { get; init; }
    public string OptionText { get; init; } = default!;
    public int VoteCount { get; init; }
}
