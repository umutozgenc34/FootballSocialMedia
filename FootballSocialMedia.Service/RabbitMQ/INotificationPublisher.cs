using FootballSocialMedia.Model.Notifications.Entities;

namespace FootballSocialMedia.Service.RabbitMQ;

public interface INotificationPublisher
{
    void SendNotification(Notification notification);
}
