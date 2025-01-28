using Core.Persistence.Entity;

namespace FootballSocialMedia.Model.Posts.Entities;

public sealed class Post : BaseEntity<Guid>, IAuditEntity
{
    public string Content { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; } = new List<string>();
    public List<string> VideoUrls { get; set; } = new List<string>();
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
