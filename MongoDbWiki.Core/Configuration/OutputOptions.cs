namespace MongoDbWiki.Core.Configuration;

public class OutputOptions
{
    public const string SectionName = "Output";
    public string MergeWithFile { get; set; }
    public string OutputFile { get; set; }
}