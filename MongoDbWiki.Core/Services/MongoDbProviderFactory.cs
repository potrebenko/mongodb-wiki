using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbWiki.Core.Configuration;

namespace MongoDbWiki.Core.Services;

public class MongoDbProviderFactory : IMongoDbProviderFactory
{
    private readonly ConnectionOptions _connectionOptions;

    public MongoDbProviderFactory(IOptions<ConnectionOptions> options)
    {
        _connectionOptions = options.Value;
    }

    public List<MongoDbProvider> CreateProviders()
    {
        var providers = new List<MongoDbProvider>();

        foreach (var mongoDbConnectionSetting in _connectionOptions.ConnectionStrings)
        {
            var mongoDbProvider = new MongoDbProvider(mongoDbConnectionSetting.ConnectionString);
            if (PingDatabase(mongoDbProvider.Database))
            {
                providers.Add(mongoDbProvider);
            }
            else
            {
                Console.WriteLine($"Warning: DB {mongoDbProvider.DatabaseName} isn't reachable: {mongoDbConnectionSetting.ConnectionString}");
            }
        }
        
        return providers;
    }

    private bool PingDatabase(IMongoDatabase? database)
    {
        try
        {
            if (database == null)
            {
                return false;
            }

            var command = new BsonDocument("ping", 1);
            database.RunCommand<BsonDocument>(command);
            return true;
        }
        catch
        {
            return false;
        }
    }
}