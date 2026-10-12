using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FootballSocialMedia.Service.RabbitMQ;

/// <summary>24 saatte bir MessageArchiverProducer'ı çalıştırır.</summary>
public class MessageArchiverBackgroundService(MessageArchiverProducer producer, ILogger<MessageArchiverBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ExecuteAsync ilk await'e kadar uygulamanın açılışını bloklar; DB hazır olsun diye biraz bekle.
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await producer.ArchiveOldMessagesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Mesaj arşivleme başarısız.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
