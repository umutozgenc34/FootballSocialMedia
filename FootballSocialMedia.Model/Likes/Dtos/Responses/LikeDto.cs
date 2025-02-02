namespace FootballSocialMedia.Model.Likes.Dtos.Responses;

public sealed record LikeDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = null!;
    public Guid? PostId { get; init; }
    public Guid? CommentId { get; init; }
}