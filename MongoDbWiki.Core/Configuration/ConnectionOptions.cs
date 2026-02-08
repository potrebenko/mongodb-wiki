namespace MongoDbWiki.Core.Configuration;

public class ConnectionOptions
{
    public const string SectionName = "ConnectionSettings";
    public List<MongoDbConnectionString> ConnectionStrings { get; set; }
    public int MaximumDocumentsToExamine { get; set; }
}

public class MongoDbConnectionString
{
    public string Name { get; set; }
    public string ConnectionString { get; set; }
}