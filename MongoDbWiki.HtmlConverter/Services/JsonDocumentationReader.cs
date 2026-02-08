using System.Text.Json;
using MongoDbWiki.Core.Converters;
using MongoDbWiki.Core.Models;

namespace MongoDbWiki.HtmlConverter.Services;

public class JsonDocumentationReader : IJsonDocumentationReader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new BsonTypeJsonConverter() }
    };

    public DatabaseDocumentation ReadFromFile(string path)
    {
        var json = File.ReadAllText(path);
        return ReadFromString(json);
    }

    public DatabaseDocumentation ReadFromString(string json)
    {
        return JsonSerializer.Deserialize<DatabaseDocumentation>(json, SerializerOptions)
               ?? throw new InvalidOperationException("Failed to deserialize JSON documentation: result was null");
    }
}
