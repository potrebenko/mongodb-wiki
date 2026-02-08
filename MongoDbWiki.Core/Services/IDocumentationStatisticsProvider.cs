using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public interface IDocumentationStatisticsProvider
{
    DocumentationStatistics CreateStatistics(DatabaseDocumentation? oldDocumentation, List<DatabaseSchema> newSchemas);
}