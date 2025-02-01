using FootballSocialMedia.Model.Comments.Dtos.Responses;

namespace FootballSocialMedia.Model.Posts.Dtos.Responses;

public sealed record PostWithCommentsResponse
{
    public Guid Id { get; init; }
    public string Content { get; init; }
    public List<string> ImageUrls { get; init; }
    public List<string> VideoUrls { get; init; }
    public string UserId { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
    public List<CommentDto> Comments { get; init; }
}
    
