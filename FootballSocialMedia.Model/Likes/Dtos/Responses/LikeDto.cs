namespace FootballSocialMedia.Model.Likes.Dtos.Responses;

public sealed record LikeDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = null!;
    public string Username { get; init; } = null!;
    public Guid? PostId { get; init; }
    public Guid? CommentId { get; init; }
    public string? PostContent { get; init; }
    public string? CommentContent { get; init; }
    /// <summary>İşlemden sonraki güncel beğeni sayısı</summary>
    public int LikeCount { get; init; }
}
