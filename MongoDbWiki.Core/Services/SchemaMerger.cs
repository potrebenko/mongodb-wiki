using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public class SchemaMerger : ISchemaMerger
{
    private readonly IDocumentationStatisticsProvider _statisticsProvider;

    public SchemaMerger(IDocumentationStatisticsProvider statisticsProvider)
    {
        _statisticsProvider = statisticsProvider;
    }

    public DocumentationStatistics MergeSchemasWithDescriptions(List<DatabaseSchema> newSchemas, DatabaseDocumentation? oldSchemas)
    {
        var schemaStatistics = _statisticsProvider.CreateStatistics(oldSchemas, newSchemas);
        var totalFoundDescriptions = 0;

        var descriptionsByField = new Dictionary<string, string>();
        if (oldSchemas != null)
        {
            foreach (var schema in oldSchemas.Schemas)
            {
                descriptionsByField.Add(schema.DatabaseName, schema.Description);
                BuildDescriptionDictionary(schema.Collections, descriptionsByField);
            }
        }

        foreach (var database in newSchemas)
        {
            if (descriptionsByField.TryGetValue(database.DatabaseName, out var databaseDescription))
            { 
                database.SetDescription(databaseDescription);
            }
            totalFoundDescriptions += ApplyDescriptions(database.Collections, descriptionsByField);
        }

        schemaStatistics.TotalItemsWithDescription = totalFoundDescriptions;
        return schemaStatistics;
    }

    private static int ApplyDescriptions(List<DocumentNode> nodes, Dictionary<string, string> descriptionsByField)
    {
        var count = 0;
        foreach (var node in nodes)
        {
            if (descriptionsByField.TryGetValue(node.FieldNamespace, out var description) && !string.IsNullOrEmpty(description))
            {
                node.SetDescription(description);
                count++;
            }

            count += ApplyDescriptions(node.ChildNodes, descriptionsByField);
        }

        return count;
    }

    private static void BuildDescriptionDictionary(List<DocumentNode> nodes, Dictionary<string, string> dictionary)
    {
        foreach (var node in nodes)
        {
            dictionary.TryAdd(node.FieldNamespace, node.Description);
            BuildDescriptionDictionary(node.ChildNodes, dictionary);
        }
    }
}