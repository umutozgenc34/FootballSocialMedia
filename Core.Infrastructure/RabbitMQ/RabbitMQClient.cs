using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Core.Infrastructure.RabbitMQ;

public class RabbitMQClient : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQClient(IConfiguration configuration)
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(configuration.GetConnectionString("RabbitMQ"))
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public IModel GetChannel() => _channel;

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}