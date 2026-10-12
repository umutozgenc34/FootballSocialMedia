namespace FootballSocialMedia.Model.Follows.Dtos.Responses;

public sealed record FollowDto
{
    public string FollowerId { get; init; } = default!;
    public string? FollowerUserName { get; init; }
    public string? FollowerProfileImageUrl { get; init; }
    public string FollowingId { get; init; } = default!;
    public string? FollowingUserName { get; init; }
    public string? FollowingProfileImageUrl { get; init; }
    public DateTime CreatedTime { get; init; }
}
