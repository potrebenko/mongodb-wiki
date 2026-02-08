using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public interface IOutputBuilder
{
    (string output, DocumentationStatistics statistics) BuildOutput(List<DatabaseSchema> schemas);
}