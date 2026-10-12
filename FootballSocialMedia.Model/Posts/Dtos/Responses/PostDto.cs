namespace FootballSocialMedia.Model.Posts.Dtos.Responses;

public sealed record PostDto
{
    public Guid Id { get; init; }
    public string Content { get; init; } = default!;
    public List<string> ImageUrls { get; init; } = new();
    public List<string> VideoUrls { get; init; } = new();
    public string UserId { get; init; } = default!;
    public string Username { get; init; } = default!;
    public string? ProfileImageUrl { get; init; }
    public string? FavoriteTeam { get; init; }
    public int LikeCount { get; init; }
    public int CommentCount { get; init; }
    public bool IsLikedByMe { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
}
