using Core.Persistence.Entity;
using FootballSocialMedia.Model.Users.Entities;

namespace FootballSocialMedia.Model.Follows.Entities;

public class Follow : IAuditEntity
{
    public string FollowerId { get; set; }
    public string FollowingId { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    public virtual User Follower { get; set; }
    public virtual User Following { get; set; }
}
