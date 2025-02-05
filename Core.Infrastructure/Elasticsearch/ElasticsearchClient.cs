using Microsoft.Extensions.Options;
using Nest;

namespace Core.Infrastructure.Elasticsearch;

public class ElasticsearchClient
{
    private readonly IElasticClient _elasticClient;

    public ElasticsearchClient(IOptions<ElasticsearchSettings> elasticsearchSettings)
    {
        var settings = new ConnectionSettings(new Uri(elasticsearchSettings.Value.Url))
            .DefaultIndex("fsocial_media"); 

        _elasticClient = new ElasticClient(settings);

    }

    public IElasticClient GetClient() => _elasticClient;
}
