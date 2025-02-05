using FootballSocialMedia.Model.Notifications.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Service.Notifications.Abstracts;
using MongoDB.Driver;

namespace FootballSocialMedia.Service.Notifications.Concretes;

public class NotificationService : INotificationService
{
    private readonly IMongoCollection<Notification> _notificationCollection;

    public NotificationService(MongoDbContext mongoDbContext)
    {
        _notificationCollection = mongoDbContext.GetDatabase().GetCollection<Notification>("Notifications");
    }

    public async Task<List<Notification>> GetNotificationsByUserIdAsync(string userId)
    {
        var filter = Builders<Notification>.Filter.Eq(n => n.UserId, userId);
        var notifications = await _notificationCollection.Find(filter).ToListAsync();
        return notifications;
    }
    public async Task MarkAsReadAsync(string notificationId)
    {
        var filter = Builders<Notification>.Filter.Eq(n => n.Id, notificationId);
        var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
        await _notificationCollection.UpdateOneAsync(filter, update);
    }
}
