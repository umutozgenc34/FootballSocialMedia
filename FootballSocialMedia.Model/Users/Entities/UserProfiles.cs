using Core.Persistence.Entity;

namespace FootballSocialMedia.Model.Users.Entities;

public sealed class UserProfile : BaseEntity<Guid>
{
    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    public string? FavoriteTeam { get; set; }
    public string? Bio { get; set; } 
    public string? ProfileImageUrl { get; set; }
}
