using Core.Infrastructure.RabbitMQ;
using FootballSocialMedia.Model.Notifications.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace FootballSocialMedia.Service.RabbitMQ;

public class NotificationPublisher : INotificationPublisher
{
    public const string QueueName = "notifications";

    private readonly IModel _channel;
    private readonly ILogger<NotificationPublisher> _logger;
    // IModel thread-safe değil, aynı anda birden fazla request publish edebilir.
    private readonly object _lock = new();

    public NotificationPublisher(RabbitMQClient rabbitMQClient, ILogger<NotificationPublisher> logger)
    {
        _logger = logger;
        _channel = rabbitMQClient.CreateChannel();
        _channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void SendNotification(Notification notification)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(notification));
            lock (_lock)
            {
                _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);
            }
        }
        catch (Exception ex)
        {
            // Bildirim gönderilemedi diye beğeni/takip işlemi başarısız olmasın
            _logger.LogWarning(ex, "Bildirim RabbitMQ'ya gönderilemedi.");
        }
    }
}
