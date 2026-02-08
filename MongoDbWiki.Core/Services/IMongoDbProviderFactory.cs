namespace MongoDbWiki.Core.Services;

public interface IMongoDbProviderFactory
{
    List<MongoDbProvider> CreateProviders();
}