using Core.Infrastructure.RabbitMQ;
using FootballSocialMedia.Model.ChatMessages.Entities;
using FootballSocialMedia.Repository.Context;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
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

                _mongoDbContext.ArchivedMessages.InsertOne(archivedMessage);
            }

            _channel.BasicAck(ea.DeliveryTag, false);

        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
}