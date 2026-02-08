using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public interface ISchemaMerger
{
    DocumentationStatistics MergeSchemasWithDescriptions(List<DatabaseSchema> newSchemas, DatabaseDocumentation? oldSchemas);
}