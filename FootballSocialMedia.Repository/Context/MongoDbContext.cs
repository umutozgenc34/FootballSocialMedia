using FootballSocialMedia.Model.ChatMessages.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace FootballSocialMedia.Repository.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        _database = client.GetDatabase("SocialMediaDb"); 
    }

    public IMongoCollection<ArchivedMessage> ArchivedMessages
        => _database.GetCollection<ArchivedMessage>("ArchivedMessages");

}