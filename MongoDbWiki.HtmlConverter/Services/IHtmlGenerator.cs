using MongoDbWiki.Core.Models;

namespace MongoDbWiki.HtmlConverter.Services;

public interface IHtmlGenerator
{
    string GenerateHtml(DatabaseDocumentation documentation, string pageTitle);
}
