using Core.Infrastructure.RabbitMQ;
using FootballSocialMedia.Repository.Context;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FootballSocialMedia.Service.RabbitMQ;

public class MessageArchiverProducer //sqlden 30 gunluk mesajları cekicek
{
    private readonly IModel _channel;
    private const string QueueName = "ArchivedMessagesQueue";
    private readonly IServiceProvider _serviceProvider; 

    public MessageArchiverProducer(RabbitMQClient rabbitMqClient, IServiceProvider serviceProvider)
    {
        _channel = rabbitMqClient.GetChannel();
        _serviceProvider = serviceProvider;

        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    public void ArchiveOldMessages()
    {
        using (var scope = _serviceProvider.CreateScope()) 
        {
            var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var oldMessages = _context.ChatMessages
                .Where(m => m.Timestamp < DateTime.UtcNow.AddDays(-30))
                .ToList();

            foreach (var message in oldMessages)
            {
                var jsonMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);
            }
        }
    }
}