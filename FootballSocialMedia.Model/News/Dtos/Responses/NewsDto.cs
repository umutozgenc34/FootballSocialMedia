namespace FootballSocialMedia.Model.News.Dtos.Responses;

public sealed record NewsDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Content { get; init; } = null!;
    public string Source { get; init; } = null!;
    public string? ImageUrl { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
    
}