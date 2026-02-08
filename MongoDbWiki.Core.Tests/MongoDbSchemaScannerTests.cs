using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDbWiki.Core.Configuration;
using MongoDbWiki.Core.Models;
using MongoDbWiki.Core.Services;

namespace MongoDbWiki.Core.Tests;

public class MongoDbSchemaScannerTests
{
    [Theory]
    [CustomAutoData]
    public void ParserDocument_ShouldReturnOnlyUniqueFields(MongoDbSchemaScanner service)
    {
        // Arrange
        var document = CreateBsonDocument();
        var collectionName = "CollectionName";
        var rootNode = new DocumentNode("root", collectionName, BsonType.Document);
        var foundFields = new HashSet<string>();
        
        
        // Act
        service.ParseDocument(document, rootNode, collectionName, foundFields);
        service.ParseDocument(document, rootNode, collectionName, foundFields);

        // Assert
        rootNode.ChildNodes[4].FieldName.Should().Be("Boolean");
        rootNode.ChildNodes[4].FieldType.Should().Be(BsonType.Boolean);
    }

    private BsonDocument CreateBsonDocument()
    {
        return new BsonDocument
        {
            { "_id", ObjectId.GenerateNewId() },
            { "Name", "Test" },
            { "Value", 3 },
            { "Time", DateTime.UtcNow },
            { "Boolean", false },
            { "TestArray", new BsonArray
            {
                new BsonDocument
                {
                    { "ArrayItem", "ItemValue" }
                }
            } },
            {
                "InnerDocument", new BsonDocument
                {
                    { "InnerName", "SomeValue" }
                }
            }
        };
    }
    
    private class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base(() =>
        {
            var fixture = new Fixture();
            Customize(fixture); 
            return fixture;
        })
        {
            
        }
        
        private static void Customize(IFixture fixture)
        {
            fixture.Customize(new AutoNSubstituteCustomization());
            fixture.Register(() => new List<MongoDbProvider>());
            fixture.Register(() => Options.Create(new ConnectionOptions()));
        }
    }
}