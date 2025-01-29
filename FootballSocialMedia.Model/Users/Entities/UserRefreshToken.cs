using Core.Persistence.Entity;

namespace FootballSocialMedia.Model.Users.Entities;

public class UserRefreshToken : BaseEntity<string>
{
    public string Code { get; set; } = default!;
    public DateTime Expiration { get; set; }
}