using Microsoft.Extensions.Options;
using Nest;

namespace Core.Infrastructure.Elasticsearch;

public class ElasticsearchClient
{
    private readonly IElasticClient _elasticClient;

    public ElasticsearchClient(IOptions<ElasticsearchSettings> elasticsearchSettings)
    {
        var settings = new ConnectionSettings(new Uri(elasticsearchSettings.Value.Url))
            .DefaultIndex("fsocial_media")
            // NEST 7.17 istemcisinin Elasticsearch 8.x ile konuşabilmesi için uyumluluk modu
            .EnableApiVersioningHeader()
            // ES kapalıysa istekler uzun süre asılı kalmasın
            .RequestTimeout(TimeSpan.FromSeconds(5));

        _elasticClient = new ElasticClient(settings);
    }

    public IElasticClient GetClient() => _elasticClient;
}
