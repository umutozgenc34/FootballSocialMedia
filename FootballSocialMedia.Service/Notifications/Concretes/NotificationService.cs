using Core.CrossCuttingConcerns.Response;
using Core.Security.Services.Abstracts;
using FootballSocialMedia.Model.Notifications.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Service.Notifications.Abstracts;
using MongoDB.Driver;
using System.Net;

namespace FootballSocialMedia.Service.Notifications.Concretes;

public class NotificationService(MongoDbContext mongoDbContext, IIdentityService identityService) : INotificationService
{
    private readonly IMongoCollection<Notification> _notifications =
        mongoDbContext.GetDatabase().GetCollection<Notification>("Notifications");

    public async Task<ServiceResult<List<Notification>>> GetMyNotificationsAsync(int take = 50)
    {
        var me = identityService.GetUserId;
        var notifications = await _notifications
            .Find(n => n.UserId == me)
            .SortByDescending(n => n.CreatedAt)
            .Limit(Math.Clamp(take, 1, 100))
            .ToListAsync();

        return ServiceResult<List<Notification>>.Success(notifications);
    }

    public async Task<ServiceResult<long>> GetUnreadCountAsync()
    {
        var me = identityService.GetUserId;
        var count = await _notifications.CountDocumentsAsync(n => n.UserId == me && !n.IsRead);
        return ServiceResult<long>.Success(count);
    }

    public async Task<ServiceResult> MarkAsReadAsync(string notificationId)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(notificationId, out _))
            return ServiceResult.Fail("Notification not found.", HttpStatusCode.NotFound);

        // Sadece kendi bildirimini okundu yapabilsin
        var me = identityService.GetUserId;
        var result = await _notifications.UpdateOneAsync(
            n => n.Id == notificationId && n.UserId == me,
            Builders<Notification>.Update.Set(n => n.IsRead, true));

        return result.MatchedCount == 0
            ? ServiceResult.Fail("Notification not found.", HttpStatusCode.NotFound)
            : ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> MarkAllAsReadAsync()
    {
        var me = identityService.GetUserId;
        await _notifications.UpdateManyAsync(
            n => n.UserId == me && !n.IsRead,
            Builders<Notification>.Update.Set(n => n.IsRead, true));

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
