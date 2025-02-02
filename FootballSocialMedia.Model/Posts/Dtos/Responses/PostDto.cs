namespace FootballSocialMedia.Model.Posts.Dtos.Responses;

public sealed record PostDto()
{
    public Guid Id { get; init; }
    public string Content { get; init; }
    public List<string> ImageUrls { get; init; }
    public List<string> VideoUrls { get; init; }
    public string Username { get; set; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
}

