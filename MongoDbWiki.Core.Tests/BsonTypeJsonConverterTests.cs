using System.Text.Json;
using FluentAssertions;
using MongoDB.Bson;
using MongoDbWiki.Core.Converters;
using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Tests;

public class BsonTypeJsonConverterTests
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new BsonTypeJsonConverter() }
    };

    [Theory]
    [InlineData(BsonType.Document, "\"Document\"")]
    [InlineData(BsonType.String, "\"String\"")]
    [InlineData(BsonType.Boolean, "\"Boolean\"")]
    [InlineData(BsonType.Int32, "\"Int32\"")]
    [InlineData(BsonType.Int64, "\"Int64\"")]
    [InlineData(BsonType.Double, "\"Double\"")]
    [InlineData(BsonType.Array, "\"Array\"")]
    [InlineData(BsonType.ObjectId, "\"ObjectId\"")]
    [InlineData(BsonType.DateTime, "\"DateTime\"")]
    [InlineData(BsonType.Decimal128, "\"Decimal128\"")]
    [InlineData(BsonType.Null, "\"Null\"")]
    public void Write_ShouldSerializeBsonTypeAsReadableString(BsonType bsonType, string expectedJson)
    {
        // Act
        var json = JsonSerializer.Serialize(bsonType, _options);

        // Assert
        json.Should().Be(expectedJson);
    }

    [Theory]
    [InlineData("\"Document\"", BsonType.Document)]
    [InlineData("\"String\"", BsonType.String)]
    [InlineData("\"Boolean\"", BsonType.Boolean)]
    [InlineData("\"Int32\"", BsonType.Int32)]
    [InlineData("\"Array\"", BsonType.Array)]
    [InlineData("\"ObjectId\"", BsonType.ObjectId)]
    public void Read_ShouldDeserializeStringToBsonType(string json, BsonType expectedType)
    {
        // Act
        var result = JsonSerializer.Deserialize<BsonType>(json, _options);

        // Assert
        result.Should().Be(expectedType);
    }

    [Fact]
    public void Write_ShouldProduceReadableFieldType_InDocumentNode()
    {
        // Arrange
        var node = new DocumentNode("root.Name", "Name", BsonType.String);

        // Act
        var json = JsonSerializer.Serialize(node, _options);

        // Assert
        json.Should().Contain("\"FieldType\":\"String\"");
    }
}