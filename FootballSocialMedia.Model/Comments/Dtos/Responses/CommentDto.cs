namespace FootballSocialMedia.Model.Comments.Dtos.Responses;

public sealed record CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; init; } 
    public string Username { get; init; } 
    public string PostContent { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
}
