using FootballSocialMedia.Model.Comments.Dtos.Responses;

namespace FootballSocialMedia.Model.Posts.Dtos.Responses;

public sealed record PostWithCommentsResponse
{
    public Guid Id { get; init; }
    public string Content { get; init; } = default!;
    public List<string> ImageUrls { get; init; } = new();
    public List<string> VideoUrls { get; init; } = new();
    public string Username { get; init; } = default!;
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
    public List<CommentDto> Comments { get; init; } = new();
}
