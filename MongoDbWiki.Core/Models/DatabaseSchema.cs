namespace MongoDbWiki.Core.Models;

public class DatabaseSchema
{
    public string DatabaseName { get; private set; }
    public string Description { get; private set; }
    public List<DocumentNode> Collections { get; private set; }

    public DatabaseSchema(string databaseName, string description, List<DocumentNode> collections)
    {
        DatabaseName = databaseName;
        Collections = collections;
        Description = description;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }
}
