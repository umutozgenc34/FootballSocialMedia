namespace FootballSocialMedia.Model.Likes.Dtos.Responses;

public sealed record LikeDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = null!;
    public string? PostContent { get; init; }
    public string? CommentContent { get; init; }
}