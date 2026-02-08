using MongoDbWiki.Core.Models;

namespace MongoDbWiki.HtmlConverter.Services;

public interface IJsonDocumentationReader
{
    DatabaseDocumentation ReadFromFile(string path);
    DatabaseDocumentation ReadFromString(string json);
}
