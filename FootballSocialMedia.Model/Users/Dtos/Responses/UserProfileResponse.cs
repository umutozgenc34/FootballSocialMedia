namespace FootballSocialMedia.Model.Users.Dtos.Responses;

public sealed record UserProfileResponse
{
    public string Id { get; init; } = default!;
    public string UserName { get; init; } = default!;
    /// <summary>Sadece kendi profilinde dolu gelir.</summary>
    public string? Email { get; init; }
    public string? FavoriteTeam { get; init; }
    public string? Bio { get; init; }
    public string? ProfileImageUrl { get; init; }
    public int PostCount { get; init; }
    public int FollowerCount { get; init; }
    public int FollowingCount { get; init; }
    public bool IsMe { get; init; }
    public bool IsFollowedByMe { get; init; }
    public List<string> Roles { get; init; } = new();
    public DateTime CreatedTime { get; init; }
}
