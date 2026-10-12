namespace FootballSocialMedia.Model.Polls.Dtos.Responses;

public sealed record PollDto
{
    public Guid Id { get; init; }
    public string Question { get; init; } = default!;
    public List<PollOptionDto> Options { get; init; } = new();
    public int TotalVotes { get; init; }
    /// <summary>Giriş yapan kullanıcının oy verdiği seçenek (oy vermediyse null)</summary>
    public Guid? MyVoteOptionId { get; init; }
    public bool IsExpired { get; init; }
    public DateTime ExpiryDate { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
}
