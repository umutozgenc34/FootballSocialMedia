namespace FootballSocialMedia.Model.Posts.Dtos.Responses;

public sealed record PostDto()
{
    public Guid Id { get; init; }
    public string Content { get; init; }
    public string ImageUrl { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
}

