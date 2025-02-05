using Core.Infrastructure.RabbitMQ;
using FootballSocialMedia.Model.Notifications.Entities;
using FootballSocialMedia.Repository.Context;
using MongoDB.Driver;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FootballSocialMedia.Service.RabbitMQ;
public class NotificationConsumer
{
    private readonly IModel _channel;
    private readonly IMongoCollection<Notification> _notificationCollection;

    public NotificationConsumer(RabbitMQClient rabbitMQClient, MongoDbContext mongoDbContext)
    {
        _channel = rabbitMQClient.GetChannel();  
        _notificationCollection = mongoDbContext.GetDatabase().GetCollection<Notification>("Notifications");

        _channel.QueueDeclare(queue: "notifications", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void StartListening()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var notification = JsonConvert.DeserializeObject<Notification>(message);

            if (notification != null)
            {
                _notificationCollection.InsertOne(notification);
                Console.WriteLine($"[x] Received: {message}");
            }
        };

        _channel.BasicConsume(queue: "notifications", autoAck: true, consumer: consumer);
    }
}