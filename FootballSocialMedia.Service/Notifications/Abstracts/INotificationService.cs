using Core.CrossCuttingConcerns.Response;
using FootballSocialMedia.Model.Notifications.Entities;

namespace FootballSocialMedia.Service.Notifications.Abstracts;

public interface INotificationService
{
    Task<ServiceResult<List<Notification>>> GetMyNotificationsAsync(int take = 50);
    Task<ServiceResult<long>> GetUnreadCountAsync();
    Task<ServiceResult> MarkAsReadAsync(string notificationId);
    Task<ServiceResult> MarkAllAsReadAsync();
}
