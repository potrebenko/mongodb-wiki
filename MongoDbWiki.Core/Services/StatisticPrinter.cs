using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Services;

public class StatisticPrinter : IStatisticPrinter
{
    public void Print(DocumentationStatistics statistics)
    {
        Console.WriteLine("DB Statistics");
        Console.WriteLine("Old schema:");
        Console.WriteLine($"  Collections: {statistics.TotalCollectionsOldSchema}");
        Console.WriteLine($"  Fields: {statistics.TotalFieldsInOldSchema}");
        Console.WriteLine("New schema:");
        Console.WriteLine($"  Collections: {statistics.TotalCollectionsNewSchema}");
        Console.WriteLine($"  Fields: {statistics.TotalFieldsInNewSchema}");
        Console.WriteLine($"  Coverage: {statistics.CalculateCoverage()}%");
    }
}
