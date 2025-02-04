using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FootballSocialMedia.Model.ChatMessages.Entities;

public class ArchivedMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } 

    [BsonElement("SenderId")]
    public string SenderId { get; set; } 

    [BsonElement("ReceiverId")]
    public string ReceiverId { get; set; } 

    [BsonElement("Message")]
    public string Message { get; set; } 

    [BsonElement("Timestamp")]
    public DateTime Timestamp { get; set; } 
}