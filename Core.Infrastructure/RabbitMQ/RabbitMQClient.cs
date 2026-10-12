using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Core.Infrastructure.RabbitMQ;

/// <summary>
/// Uygulama boyunca tek bir RabbitMQ bağlantısı tutar.
/// IModel (channel) thread-safe olmadığı için her publisher/consumer kendi kanalını <see cref="CreateChannel"/> ile açar.
/// </summary>
public class RabbitMQClient : IDisposable
{
    private readonly IConnection _connection;

    public RabbitMQClient(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(configuration.GetConnectionString("RabbitMQ")!),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };

        // Docker container'ları API'den biraz geç ayağa kalkabiliyor, birkaç kez dene.
        Exception? lastError = null;
        for (var attempt = 1; attempt <= 5; attempt++)
        {
            try
            {
                _connection = factory.CreateConnection();
                return;
            }
            catch (Exception ex)
            {
                lastError = ex;
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }

        throw new InvalidOperationException(
            "RabbitMQ'ya bağlanılamadı. 'docker compose up -d' ile container'ların çalıştığından emin olun.", lastError);
    }

    public IModel CreateChannel() => _connection.CreateModel();

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }
}
