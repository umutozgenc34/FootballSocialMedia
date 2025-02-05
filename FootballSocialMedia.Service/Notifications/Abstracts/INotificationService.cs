using FootballSocialMedia.Model.Notifications.Entities;

namespace FootballSocialMedia.Service.Notifications.Abstracts;

public interface INotificationService
{
    Task<List<Notification>> GetNotificationsByUserIdAsync(string userId);
    Task MarkAsReadAsync(string notificationId);

}
