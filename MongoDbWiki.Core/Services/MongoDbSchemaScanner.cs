using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbWiki.Core.Configuration;
using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public class MongoDbSchemaScanner : IMongoDbSchemaScanner
{
    private readonly int _maximumDocumentsToExamine;

    public MongoDbSchemaScanner(IOptions<ConnectionOptions> options)
    {
        _maximumDocumentsToExamine = options.Value.MaximumDocumentsToExamine;
    }

    public List<DatabaseSchema> ScanDatabaseSchema(List<MongoDbProvider> databaseProviders)
    {
        var databases = new List<DatabaseSchema>();
        foreach (var mongoDbProvider in databaseProviders)
        {
            var collections = new List<DocumentNode>();

            var databaseName = mongoDbProvider.DatabaseName;
            var databaseSchema = new DatabaseSchema(databaseName, string.Empty, collections);
            var collectionNames = GetCollectionNames(mongoDbProvider);
           
            if (collectionNames.Count == 0)
            {
                Console.WriteLine($"Warning: DB {databaseName} is empty");
                continue;
            }
            
            foreach (var collectionName in collectionNames)
            {
                var collection = ScanCollection(mongoDbProvider.Database, collectionName, _maximumDocumentsToExamine);
                collections.Add(collection);
            }
            databases.Add(databaseSchema);
        }

        return databases;
    }

    private static List<string> GetCollectionNames(MongoDbProvider mongoDbProvider)
    {
        var systemCollections = new HashSet<string> { "system.indexes", "system.users", "system.profile" };
        var collections = mongoDbProvider.Database!.ListCollectionNames().ToList();
        // Filter out system collections
        return collections.Where(c => !systemCollections.Contains(c)).ToList();
    }

    public DocumentNode ScanCollection(IMongoDatabase database, string collectionName, int maximumDocumentsToExamine)
    {
        var collection = database.GetCollection<BsonDocument>(collectionName);

        var samples = collection.Find(new BsonDocument()).Limit(maximumDocumentsToExamine).ToList();
        var foundFields = new HashSet<string>();
       
        var rootNode = new DocumentNode(collectionName, collectionName, BsonType.Document);
        foreach (var sample in samples)
        {
            ParseDocument(sample, rootNode, collectionName, foundFields);
        }

        return rootNode;
    }

    public void ParseDocument(BsonDocument document, DocumentNode node, string prefix, HashSet<string> foundFields)
    {
        foreach (var element in document.Elements)
        {
            var fieldNamespace = string.IsNullOrEmpty(prefix) ? element.Name : $"{prefix}.{element.Name}";

            if (!foundFields.Add(fieldNamespace))
            {
                continue;
            }

            var fieldName = element.Name;
            var fieldType = element.Value.BsonType;

            var innerNode = new DocumentNode(fieldNamespace, fieldName, fieldType);
            node.AddNode(innerNode);

            if (fieldType == BsonType.Document)
            {
                ParseDocument(element.Value.AsBsonDocument, innerNode, fieldNamespace, foundFields);
            }
            else if (fieldType == BsonType.Array)
            {
                var array = element.Value.AsBsonArray;
                if (array.Count > 0)
                {
                    var firstElement = array.First();
                    if (firstElement.BsonType == BsonType.Document)
                    {
                        ParseDocument(firstElement.AsBsonDocument, innerNode, $"{fieldNamespace}[]", foundFields);
                    }
                }
            }
        }
    } 
}