using FootballSocialMedia.Model.Notifications.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FootballSocialMedia.Model.Notifications.Entities;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty; 
    [BsonElement("senderId")]
    public string SenderId { get; set; } = string.Empty; 

    [BsonElement("senderUserName")]
    public string? SenderUserName { get; set; }

    [BsonElement("type")]
    public NotificationType Type { get; set; }

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty; 

    [BsonElement("postId")]
    public string? PostId { get; set; }
    [BsonElement("commentId")]
    public string? CommentId { get; set; }

    [BsonElement("isRead")]
    public bool IsRead { get; set; } = false; 

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
