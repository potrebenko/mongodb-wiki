using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public class DocumentationStatisticsProvider : IDocumentationStatisticsProvider
{
    public DocumentationStatistics CreateStatistics(DatabaseDocumentation? oldDocumentation, List<DatabaseSchema> newSchemas)
    {
        var statistics = new DocumentationStatistics();
        var totalFieldsInOldSchema = CalculateTotalFields(oldDocumentation?.Schemas ?? new List<DatabaseSchema>());
        var totalFieldsInNewSchema = CalculateTotalFields(newSchemas);
        statistics.OldVersion = oldDocumentation?.Version ?? string.Empty;
        statistics.TotalFieldsInOldSchema = totalFieldsInOldSchema;
        statistics.TotalFieldsInNewSchema = totalFieldsInNewSchema;
        statistics.TotalCollectionsOldSchema = oldDocumentation?.Schemas.Count ?? 0;
        statistics.TotalCollectionsNewSchema = newSchemas.Count;
        return statistics;
    }

    private int CalculateTotalFields(List<DatabaseSchema> nodes)
    {
        var totalFieldsInSchema = 0;
        foreach (var node in nodes)
        {
            totalFieldsInSchema += CalculateTotalFieldsByNode(node.Collections);
        }

        return totalFieldsInSchema;
    }

    private int CalculateTotalFieldsByNode(List<DocumentNode> nodes)
    {
        var totalFieldsInSchema = 0;
        foreach (var node in nodes)
        {
            totalFieldsInSchema++;
            totalFieldsInSchema += CalculateTotalFieldsByNode(node.ChildNodes);
        }

        return totalFieldsInSchema;
    }
}