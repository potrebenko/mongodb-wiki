using System.Text.Json;
using Microsoft.Extensions.Options;
using MongoDbWiki.Core.Configuration;
using MongoDbWiki.Core.Converters;
using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public class JsonOutputBuilder : IOutputBuilder
{
    private const string DocumentVersion = "1";
    private readonly ISchemaMerger _schemaMerger;
    private readonly IOptions<OutputOptions> _options;

    public JsonOutputBuilder(ISchemaMerger schemaMerger, IOptions<OutputOptions> options)
    {
        _schemaMerger = schemaMerger;
        _options = options;
    }

    public (string output, DocumentationStatistics statistics) BuildOutput(List<DatabaseSchema> schemas)
    {
        DatabaseDocumentation? originalDatabaseDocumentation = null;
        DocumentationStatistics? statistics = null;
        string title = string.Empty;
        if (File.Exists(_options.Value.MergeWithFile))
        {
            var originalSchema = File.ReadAllText(_options.Value.MergeWithFile);
            originalDatabaseDocumentation = JsonSerializer.Deserialize<DatabaseDocumentation>(originalSchema,
                new JsonSerializerOptions
                {
                    Converters = { new BsonTypeJsonConverter() }
                });
            title = originalDatabaseDocumentation?.Title ?? string.Empty;
        }

        statistics = _schemaMerger.MergeSchemasWithDescriptions(schemas, originalDatabaseDocumentation);

        var documentation = new DatabaseDocumentation(DocumentVersion, title, schemas);
        var jsonOutput = JsonSerializer.Serialize(documentation, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new BsonTypeJsonConverter() }
        });

        return (jsonOutput, statistics);
    }
}