using Microsoft.Extensions.Hosting;

namespace FootballSocialMedia.Service.RabbitMQ;

public class NotificationWorker : BackgroundService
{
    private readonly NotificationConsumer _notificationConsumer;

    public NotificationWorker(NotificationConsumer notificationConsumer)
    {
        _notificationConsumer = notificationConsumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _notificationConsumer.StartListening();
        return Task.CompletedTask;
    }
}
