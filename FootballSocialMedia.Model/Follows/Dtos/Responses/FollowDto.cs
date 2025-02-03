namespace FootballSocialMedia.Model.Follows.Dtos.Responses;

public sealed record FollowDto
{
    public string FollowerId { get; init; }
    public string FollowingId { get; init; }
    public DateTime CreatedTime { get; init; }
    public DateTime UpdatedTime { get; init; }
}
