using Core.Infrastructure.RabbitMQ;
using FootballSocialMedia.Model.Notifications.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Service.ChatHubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FootballSocialMedia.Service.RabbitMQ;

/// <summary>
/// "notifications" kuyruğunu dinler, bildirimi MongoDB'ye yazar ve
/// kullanıcı o an online ise SignalR ile anında "ReceiveNotification" olayı gönderir.
/// </summary>
public class NotificationConsumer
{
    private readonly IModel _channel;
    private readonly IMongoCollection<Notification> _notificationCollection;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(RabbitMQClient rabbitMQClient, MongoDbContext mongoDbContext,
        IHubContext<ChatHub> hubContext, ILogger<NotificationConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
        _channel = rabbitMQClient.CreateChannel();
        _notificationCollection = mongoDbContext.GetDatabase().GetCollection<Notification>("Notifications");
        _channel.QueueDeclare(queue: NotificationPublisher.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void StartListening()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            // async void event handler: exception dışarı kaçarsa process çöker, o yüzden hepsini yakalıyoruz.
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var notification = JsonConvert.DeserializeObject<Notification>(message);
                if (notification == null) return;

                await _notificationCollection.InsertOneAsync(notification);
                await _hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bildirim işlenemedi.");
            }
        };

        _channel.BasicConsume(queue: NotificationPublisher.QueueName, autoAck: true, consumer: consumer);
    }
}
