namespace FootballSocialMedia.Model.Comments.Dtos.Responses;

public sealed record CommentDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; init; }
    public string Content { get; init; } = default!;
    public string UserId { get; init; } = default!;
    public string Username { get; init; } = default!;
    public string? ProfileImageUrl { get; init; }
    public string? PostContent { get; init; }
    public int LikeCount { get; init; }
    public bool IsLikedByMe { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
}
