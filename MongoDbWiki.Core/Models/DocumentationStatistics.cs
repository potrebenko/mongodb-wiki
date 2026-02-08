namespace MongoDbWiki.Core.Models;

public class DocumentationStatistics
{
    public string OldVersion { get; set; }
    public int TotalFieldsInOldSchema { get; set; }
    public int TotalFieldsInNewSchema { get; set; }
    public int TotalCollectionsOldSchema { get; set; }
    public int TotalCollectionsNewSchema { get; set; }
    public int TotalItemsWithDescription { get; set; }

    public int CalculateCoverage()
    {
        if (TotalFieldsInNewSchema == 0)
            return 0;

        return (int)(TotalItemsWithDescription / (double)TotalFieldsInNewSchema * 100);
    }
}