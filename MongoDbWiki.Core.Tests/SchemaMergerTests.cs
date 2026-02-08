using FluentAssertions;
using MongoDB.Bson;
using MongoDbWiki.Core.Models;
using MongoDbWiki.Core.Services;
using NSubstitute;

namespace MongoDbWiki.Core.Tests;

public class SchemaMergerTests
{
    private readonly IDocumentationStatisticsProvider _statisticsProvider;
    private readonly SchemaMerger _sut;

    public SchemaMergerTests()
    {
        _statisticsProvider = Substitute.For<IDocumentationStatisticsProvider>();
        _statisticsProvider
            .CreateStatistics(Arg.Any<DatabaseDocumentation?>(), Arg.Any<List<DatabaseSchema>>())
            .Returns(new DocumentationStatistics());
        _sut = new SchemaMerger(_statisticsProvider);
    }

    [Fact]
    public void MergeSchemasWithDescriptions_ShouldApplyDescriptionFromOldSchema()
    {
        // Arrange
        var oldNode = new DocumentNode("db.users", "users", BsonType.Document, "Users collection", new List<DocumentNode>());
        var oldSchemas = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { oldNode })
        });

        var newNode = new DocumentNode("db.users", "users", BsonType.Document);
        var newSchemas = new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { newNode })
        };

        // Act
        _sut.MergeSchemasWithDescriptions(newSchemas, oldSchemas);

        // Assert
        newNode.Description.Should().Be("Users collection");
    }

    [Fact]
    public void MergeSchemasWithDescriptions_ShouldApplyDescriptionsToChildNodes()
    {
        // Arrange
        var oldChild = new DocumentNode("db.users.name", "name", BsonType.String, "User name", new List<DocumentNode>());
        var oldRoot = new DocumentNode("db.users", "users", BsonType.Document, "Users collection", new List<DocumentNode> { oldChild });
        var oldSchemas = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { oldRoot })
        });

        var newChild = new DocumentNode("db.users.name", "name", BsonType.String);
        var newRoot = new DocumentNode("db.users", "users", BsonType.Document, string.Empty, new List<DocumentNode> { newChild });
        var newSchemas = new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { newRoot })
        };

        // Act
        _sut.MergeSchemasWithDescriptions(newSchemas, oldSchemas);

        // Assert
        newRoot.Description.Should().Be("Users collection");
        newChild.Description.Should().Be("User name");
    }

    [Fact]
    public void MergeSchemasWithDescriptions_ShouldReturnCorrectDescriptionCount()
    {
        // Arrange
        var oldChild = new DocumentNode("db.users.name", "name", BsonType.String, "User name", new List<DocumentNode>());
        var oldRoot = new DocumentNode("db.users", "users", BsonType.Document, "Users collection", new List<DocumentNode> { oldChild });
        var oldSchemas = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { oldRoot })
        });

        var newChild = new DocumentNode("db.users.name", "name", BsonType.String);
        var newRoot = new DocumentNode("db.users", "users", BsonType.Document, string.Empty, new List<DocumentNode> { newChild });
        var newSchemas = new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { newRoot })
        };

        // Act
        var result = _sut.MergeSchemasWithDescriptions(newSchemas, oldSchemas);

        // Assert
        result.TotalItemsWithDescription.Should().Be(2);
    }

    [Fact]
    public void MergeSchemasWithDescriptions_WhenOldSchemasIsNull_ShouldNotApplyDescriptions()
    {
        // Arrange
        var newNode = new DocumentNode("db.users", "users", BsonType.Document);
        var newSchemas = new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { newNode })
        };

        // Act
        var result = _sut.MergeSchemasWithDescriptions(newSchemas, null);

        // Assert
        newNode.Description.Should().BeEmpty();
        result.TotalItemsWithDescription.Should().Be(0);
    }

    [Fact]
    public void MergeSchemasWithDescriptions_ShouldNotApplyEmptyDescription()
    {
        // Arrange
        var oldNode = new DocumentNode("db.users", "users", BsonType.Document, string.Empty, new List<DocumentNode>());
        var oldSchemas = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { oldNode })
        });

        var newNode = new DocumentNode("db.users", "users", BsonType.Document);
        var newSchemas = new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { newNode })
        };

        // Act
        var result = _sut.MergeSchemasWithDescriptions(newSchemas, oldSchemas);

        // Assert
        newNode.Description.Should().BeEmpty();
        result.TotalItemsWithDescription.Should().Be(0);
    }

    [Fact]
    public void MergeSchemasWithDescriptions_WhenFieldNotInOldSchema_ShouldLeaveDescriptionEmpty()
    {
        // Arrange
        var oldSchemas = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode>())
        });

        var newNode = new DocumentNode("db.users", "users", BsonType.Document);
        var newSchemas = new List<DatabaseSchema>
        {
            new("db", string.Empty, new List<DocumentNode> { newNode })
        };

        // Act
        _sut.MergeSchemasWithDescriptions(newSchemas, oldSchemas);

        // Assert
        newNode.Description.Should().BeEmpty();
    }
}