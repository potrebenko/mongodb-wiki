namespace MongoDbWiki.HtmlConverter.Configuration;

public class HtmlConverterOptions
{
    public const string SectionName = "HtmlConverter";
    public string InputJsonFile { get; set; } = "output.json";
    public string OutputHtmlFile { get; set; } = "schema.html";
    public string PageTitle { get; set; } = "MongoDB Schema Documentation";
}
