using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public interface IStatisticPrinter
{
    void Print(DocumentationStatistics statistics);
}