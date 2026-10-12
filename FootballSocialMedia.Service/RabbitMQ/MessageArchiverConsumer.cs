using Core.Infrastructure.RabbitMQ;
using FootballSocialMedia.Model.ChatMessages.Entities;
using FootballSocialMedia.Repository.Context;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FootballSocialMedia.Service.RabbitMQ;

/// <summary>Arşiv kuyruğunu dinleyip mesajları MongoDB'ye kaydeder.</summary>
public class MessageArchiverConsumer : BackgroundService
{
    private readonly IModel _channel;
    private readonly MongoDbContext _mongoDbContext;
    private readonly ILogger<MessageArchiverConsumer> _logger;

    public MessageArchiverConsumer(RabbitMQClient rabbitMqClient, MongoDbContext mongoDbContext, ILogger<MessageArchiverConsumer> logger)
    {
        _mongoDbContext = mongoDbContext;
        _logger = logger;
        _channel = rabbitMqClient.CreateChannel();
        _channel.QueueDeclare(queue: MessageArchiverProducer.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var archivedMessage = JsonSerializer.Deserialize<ArchivedMessage>(Encoding.UTF8.GetString(ea.Body.ToArray()));
                if (archivedMessage != null)
                {
                    // Eski hali Update.Set(msg => msg, ...) kullanıyordu; Mongo driver bunu çeviremeyip exception atıyordu.
                    await _mongoDbContext.ArchivedMessages.ReplaceOneAsync(
                        m => m.Id == archivedMessage.Id,
                        archivedMessage,
                        new ReplaceOptions { IsUpsert = true },
                        stoppingToken);
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arşiv mesajı MongoDB'ye yazılamadı.");
                _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
            }
        };

        _channel.BasicConsume(queue: MessageArchiverProducer.QueueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }
}
