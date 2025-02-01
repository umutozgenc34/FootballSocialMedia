namespace FootballSocialMedia.Model.Comments.Dtos.Responses;

public sealed record CommentDto
{
    public string Content { get; init; } 
    public string UserId { get; init; } 
    public Guid PostId { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
}
