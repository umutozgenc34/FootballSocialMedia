using Core.Infrastructure.RabbitMQ;
using FootballSocialMedia.Model.ChatMessages.Entities;
using FootballSocialMedia.Repository.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FootballSocialMedia.Service.RabbitMQ;

/// <summary>SQL'deki 30 günden eski mesajları RabbitMQ üzerinden MongoDB arşivine (cold storage) taşır.</summary>
public class MessageArchiverProducer
{
    public const string QueueName = "ArchivedMessagesQueue";

    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageArchiverProducer> _logger;

    public MessageArchiverProducer(RabbitMQClient rabbitMqClient, IServiceProvider serviceProvider, ILogger<MessageArchiverProducer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _channel = rabbitMqClient.CreateChannel();
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    public async Task ArchiveOldMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var threshold = DateTime.UtcNow.AddDays(-30);
        var oldMessages = context.ChatMessages.Where(m => m.Timestamp < threshold).ToList();
        if (oldMessages.Count == 0) return;

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        foreach (var message in oldMessages)
        {
            var archived = new ArchivedMessage
            {
                Id = message.Id.ToString(),
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Message = message.Message,
                Timestamp = message.Timestamp
            };
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(archived));
            _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: properties, body: body);
        }

        // Eskiden mesajlar SQL'den silinmiyordu, her gün aynı mesajlar tekrar arşivleniyordu.
        context.ChatMessages.RemoveRange(oldMessages);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Count} eski mesaj arşive taşındı.", oldMessages.Count);
    }
}
