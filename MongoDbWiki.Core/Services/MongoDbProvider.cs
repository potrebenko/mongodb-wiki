using MongoDB.Driver;

namespace MongoDbWiki.Core.Services;

public class MongoDbProvider
{
    public IMongoDatabase? Database { get; }
    public string DatabaseName { get; private set; }
    
    public MongoDbProvider(string connectionString)
    {
        var url = MongoUrl.Create(connectionString);
        DatabaseName = url.DatabaseName;
        var client = new MongoClient(url);
        Database = client.GetDatabase(DatabaseName);
    }
}