using FluentAssertions;
using MongoDB.Bson;
using MongoDbWiki.Core.Models;
using MongoDbWiki.Core.Services;

namespace MongoDbWiki.Core.Tests;

public class DocumentationStatisticsProviderTests
{
    private readonly DocumentationStatisticsProvider _sut = new();

    [Fact]
    public void CreateStatistics_ShouldCountFieldsInNewSchema()
    {
        // Arrange
        var node1 = new DocumentNode("col.name", "name", BsonType.String);
        var node2 = new DocumentNode("col.age", "age", BsonType.Int32);
        var collection = new DocumentNode("col", "col", BsonType.Document, string.Empty, new List<DocumentNode> { node1, node2 });
        var newSchemas = new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { collection })
        };

        // Act
        var result = _sut.CreateStatistics(null, newSchemas);

        // Assert
        result.TotalFieldsInNewSchema.Should().Be(3);
    }

    [Fact]
    public void CreateStatistics_ShouldCountNestedFields()
    {
        // Arrange
        var innerField = new DocumentNode("col.address.street", "street", BsonType.String);
        var address = new DocumentNode("col.address", "address", BsonType.Document, string.Empty, new List<DocumentNode> { innerField });
        var collection = new DocumentNode("col", "col", BsonType.Document, string.Empty, new List<DocumentNode> { address });
        var newSchemas = new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { collection })
        };

        // Act
        var result = _sut.CreateStatistics(null, newSchemas);

        // Assert
        result.TotalFieldsInNewSchema.Should().Be(3);
    }

    [Fact]
    public void CreateStatistics_ShouldCountFieldsInOldSchema()
    {
        // Arrange
        var oldNode = new DocumentNode("col.name", "name", BsonType.String);
        var oldCollection = new DocumentNode("col", "col", BsonType.Document, string.Empty, new List<DocumentNode> { oldNode });
        var oldDocumentation = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { oldCollection })
        });

        // Act
        var result = _sut.CreateStatistics(oldDocumentation, new List<DatabaseSchema>());

        // Assert
        result.TotalFieldsInOldSchema.Should().Be(2);
        result.OldVersion.Should().Be("1");
    }

    [Fact]
    public void CreateStatistics_WhenOldDocumentationIsNull_ShouldReturnZeroForOldFields()
    {
        // Act
        var result = _sut.CreateStatistics(null, new List<DatabaseSchema>());

        // Assert
        result.TotalFieldsInOldSchema.Should().Be(0);
        result.TotalCollectionsOldSchema.Should().Be(0);
        result.OldVersion.Should().BeEmpty();
    }

    [Fact]
    public void CreateStatistics_ShouldCountCollections()
    {
        // Arrange
        var col1 = new DocumentNode("users", "users", BsonType.Document);
        var col2 = new DocumentNode("orders", "orders", BsonType.Document);
        var newSchemas = new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { col1, col2 })
        };

        var oldCol = new DocumentNode("users", "users", BsonType.Document);
        var oldDocumentation = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { oldCol })
        });

        // Act
        var result = _sut.CreateStatistics(oldDocumentation, newSchemas);

        // Assert
        result.TotalCollectionsNewSchema.Should().Be(1);
        result.TotalCollectionsOldSchema.Should().Be(1);
    }
}