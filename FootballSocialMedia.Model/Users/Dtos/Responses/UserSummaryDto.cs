namespace FootballSocialMedia.Model.Users.Dtos.Responses;

public sealed record UserSummaryDto
{
    public string Id { get; init; } = default!;
    public string UserName { get; init; } = default!;
    public string? ProfileImageUrl { get; init; }
    public string? FavoriteTeam { get; init; }
}
