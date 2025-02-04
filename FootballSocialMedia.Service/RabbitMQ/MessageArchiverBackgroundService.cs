using Microsoft.Extensions.Hosting;

namespace FootballSocialMedia.Service.RabbitMQ;

public class MessageArchiverBackgroundService : BackgroundService //24saatte bir messagearchiverproduceri cagiricak
{
    private readonly MessageArchiverProducer _producer;

    public MessageArchiverBackgroundService(MessageArchiverProducer producer)
    {
        _producer = producer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _producer.ArchiveOldMessages();
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken); 
        }
    }
}