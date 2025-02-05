using Core.Infrastructure.RabbitMQ;
using FootballSocialMedia.Model.Notifications.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace FootballSocialMedia.Service.RabbitMQ;

public class NotificationPublisher
{
    private readonly IModel _channel;

    public NotificationPublisher(RabbitMQClient rabbitMQClient)
    {
        _channel = rabbitMQClient.GetChannel(); 
        _channel.QueueDeclare(queue: "notifications", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void SendNotification(Notification notification)
    {
        var message = JsonConvert.SerializeObject(notification);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "", routingKey: "notifications", basicProperties: null, body: body);
        Console.WriteLine($"[x] Sent: {message}");
    }
}