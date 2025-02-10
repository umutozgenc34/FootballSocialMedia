namespace FootballSocialMedia.Model.Polls.Dtos.Responses;

public sealed record PollDto
{
    public Guid Id { get; init; }
    public string Question { get; init; }
    public List<PollOptionDto> Options { get; init; }
    public DateTime ExpiryDate { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
}
