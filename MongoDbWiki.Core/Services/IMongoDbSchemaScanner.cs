using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public interface IMongoDbSchemaScanner
{
    DocumentNode ScanCollection(IMongoDatabase database, string collectionName, int maximumDocumentsToExamine);
    void ParseDocument(BsonDocument document, DocumentNode node, string prefix, HashSet<string> foundFields);
    List<DatabaseSchema> ScanDatabaseSchema(List<MongoDbProvider> databaseProviders);
}