using Core.Infrastructure.RabbitMQ;
using FootballSocialMedia.Model.ChatMessages.Entities;
using FootballSocialMedia.Repository.Context;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FootballSocialMedia.Service.RabbitMQ;

public class MessageArchiverConsumer : BackgroundService //mesajları dinleyip mongoya kaydedecek
{
    private readonly IModel _channel;
    private readonly MongoDbContext _mongoDbContext;
    private const string QueueName = "ArchivedMessagesQueue";

    public MessageArchiverConsumer(RabbitMQClient rabbitMqClient, MongoDbContext mongoDbContext)
    {
        _channel = rabbitMqClient.GetChannel();
        _mongoDbContext = mongoDbContext;

        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var jsonMessage = Encoding.UTF8.GetString(body);

            var archivedMessage = JsonSerializer.Deserialize<ArchivedMessage>(jsonMessage);

            if (archivedMessage != null)
            {

                if (string.IsNullOrEmpty(archivedMessage.Id))
                {
                    archivedMessage.Id = ObjectId.GenerateNewId().ToString();
                }
                var filter = Builders<ArchivedMessage>.Filter.Eq(msg => msg.Id, archivedMessage.Id);
                var update = Builders<ArchivedMessage>.Update.Set(msg => msg, archivedMessage);

                await _mongoDbContext.ArchivedMessages.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
            }

            _channel.BasicAck(ea.DeliveryTag, false);

        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        await Task.Delay(-1, stoppingToken);
    }
}