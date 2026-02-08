using System.Text.Json.Serialization;

namespace MongoDbWiki.Core.Models;

public class DatabaseDocumentation
{
    public string Version { get; private set; }
    public string Title { get; private set; }
    public List<DatabaseSchema> Schemas { get; private set; }

    [JsonConstructor]
    public DatabaseDocumentation(string version, string title, List<DatabaseSchema> schemas)
    {
        Version = version;
        Schemas = schemas;
        Title = title;
    }
}