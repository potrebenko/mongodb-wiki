using FluentAssertions;
using MongoDB.Bson;
using MongoDbWiki.HtmlConverter.Services;

namespace MongoDbWiki.HtmlConverter.Tests;

public class BsonTypeColorProviderTests
{
    private readonly BsonTypeColorProvider _sut = new();

    [Theory]
    [InlineData(BsonType.String, "type-string")]
    [InlineData(BsonType.Int32, "type-int32")]
    [InlineData(BsonType.Document, "type-document")]
    [InlineData(BsonType.Array, "type-array")]
    [InlineData(BsonType.ObjectId, "type-objectid")]
    [InlineData(BsonType.Boolean, "type-boolean")]
    [InlineData(BsonType.DateTime, "type-datetime")]
    public void GetCssClass_ShouldReturnLowercaseTypePrefix(BsonType bsonType, string expectedClass)
    {
        // Act
        var result = _sut.GetCssClass(bsonType);

        // Assert
        result.Should().Be(expectedClass);
    }

    [Theory]
    [InlineData(BsonType.String, "#2e7d32")]
    [InlineData(BsonType.Int32, "#1565c0")]
    [InlineData(BsonType.Int64, "#1565c0")]
    [InlineData(BsonType.Double, "#1565c0")]
    [InlineData(BsonType.Decimal128, "#1565c0")]
    [InlineData(BsonType.Boolean, "#7b1fa2")]
    [InlineData(BsonType.DateTime, "#e65100")]
    [InlineData(BsonType.ObjectId, "#795548")]
    [InlineData(BsonType.Document, "#455a64")]
    [InlineData(BsonType.Array, "#00695c")]
    [InlineData(BsonType.Null, "#9e9e9e")]
    public void GetColor_ShouldReturnExpectedColorForType(BsonType bsonType, string expectedColor)
    {
        // Act
        var result = _sut.GetColor(bsonType);

        // Assert
        result.Should().Be(expectedColor);
    }

    [Fact]
    public void GetCssClass_ShouldAlwaysStartWithTypePrefix()
    {
        // Act
        var result = _sut.GetCssClass(BsonType.String);

        // Assert
        result.Should().StartWith("type-");
    }
}
