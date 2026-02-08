using FluentAssertions;
using MongoDB.Bson;
using MongoDbWiki.HtmlConverter.Services;

namespace MongoDbWiki.HtmlConverter.Tests;

public class JsonDocumentationReaderTests
{
    private readonly JsonDocumentationReader _sut = new();

    [Fact]
    public void ReadFromString_ShouldDeserializeValidJson()
    {
        // Arrange
        var json = """
        {
            "Version": "1",
            "Title": "",
            "Schemas": [
                {
                    "DatabaseName": "TestDb",
                    "Description": "",
                    "Collections": []
                }
            ]
        }
        """;

        // Act
        var result = _sut.ReadFromString(json);

        // Assert
        result.Version.Should().Be("1");
        result.Schemas.Should().HaveCount(1);
        result.Schemas[0].DatabaseName.Should().Be("TestDb");
    }

    [Fact]
    public void ReadFromString_ShouldThrowOnInvalidJson()
    {
        // Arrange
        var invalidJson = "not valid json";

        // Act
        var act = () => _sut.ReadFromString(invalidJson);

        // Assert
        act.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public void ReadFromString_ShouldPreserveNestedStructure()
    {
        // Arrange
        var json = """
        {
            "Version": "1",
            "Title": "",
            "Schemas": [
                {
                    "DatabaseName": "TestDb",
                    "Description": "Test database",
                    "Collections": [
                        {
                            "FieldNamespace": "users",
                            "FieldName": "users",
                            "FieldType": "Document",
                            "Description": "Users collection",
                            "ChildNodes": [
                                {
                                    "FieldNamespace": "users.name",
                                    "FieldName": "name",
                                    "FieldType": "String",
                                    "Description": "User name",
                                    "ChildNodes": []
                                },
                                {
                                    "FieldNamespace": "users.age",
                                    "FieldName": "age",
                                    "FieldType": "Int32",
                                    "Description": "",
                                    "ChildNodes": []
                                }
                            ]
                        }
                    ]
                }
            ]
        }
        """;

        // Act
        var result = _sut.ReadFromString(json);

        // Assert
        var collection = result.Schemas[0].Collections[0];
        collection.FieldName.Should().Be("users");
        collection.FieldType.Should().Be(BsonType.Document);
        collection.ChildNodes.Should().HaveCount(2);
        collection.ChildNodes[0].FieldName.Should().Be("name");
        collection.ChildNodes[0].FieldType.Should().Be(BsonType.String);
        collection.ChildNodes[1].FieldName.Should().Be("age");
        collection.ChildNodes[1].FieldType.Should().Be(BsonType.Int32);
    }
}
