using System.Net;
using System.Text;
using MongoDbWiki.Core.Models;

namespace MongoDbWiki.HtmlConverter.Services;

public class HtmlGenerator : IHtmlGenerator
{
    private readonly IBsonTypeColorProvider _colorProvider;

    public HtmlGenerator(IBsonTypeColorProvider colorProvider)
    {
        _colorProvider = colorProvider;
    }

    public string GenerateHtml(DatabaseDocumentation documentation, string pageTitle)
    {
        var sb = new StringBuilder();
        var stats = CalculateStatistics(documentation);
        var title = !string.IsNullOrEmpty(documentation.Title) ? documentation.Title : pageTitle;

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        AppendHead(sb, title);
        sb.AppendLine("<body>");
        sb.AppendLine($"<h1>{WebUtility.HtmlEncode(title)}</h1>");
        AppendStatistics(sb, stats);
        AppendSchemas(sb, documentation.Schemas);
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    private void AppendHead(StringBuilder sb, string pageTitle)
    {
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"UTF-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine($"<title>{WebUtility.HtmlEncode(pageTitle)}</title>");
        AppendStyles(sb);
        sb.AppendLine("</head>");
    }

    private void AppendStyles(StringBuilder sb)
    {
        sb.AppendLine("<style>");
        sb.AppendLine("body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; max-width: 960px; margin: 0 auto; padding: 20px; background: #fafafa; color: #333; }");
        sb.AppendLine("h1 { color: #1a1a1a; border-bottom: 2px solid #e0e0e0; padding-bottom: 10px; }");
        sb.AppendLine(".stats { background: #fff; border: 1px solid #e0e0e0; border-radius: 6px; padding: 16px; margin-bottom: 24px; display: flex; gap: 24px; flex-wrap: wrap; }");
        sb.AppendLine(".stat-item { text-align: center; }");
        sb.AppendLine(".stat-value { font-size: 1.5em; font-weight: bold; color: #1565c0; }");
        sb.AppendLine(".stat-label { font-size: 0.85em; color: #757575; }");
        sb.AppendLine("details { margin-left: 20px; }");
        sb.AppendLine("summary { cursor: pointer; padding: 4px 0; font-weight: bold; }");
        sb.AppendLine("summary:hover { color: #1565c0; }");
        sb.AppendLine(".database > summary { font-size: 1.2em; color: #1a1a1a; background: #fff; padding: 8px 12px; border: 1px solid #e0e0e0; border-radius: 4px; margin: 8px 0; }");
        sb.AppendLine(".collection > summary { font-size: 1.05em; color: #333; }");
        sb.AppendLine(".field { margin-left: 20px; padding: 3px 0; }");
        sb.AppendLine(".type-badge { display: inline-block; padding: 1px 8px; border-radius: 10px; font-size: 0.8em; font-weight: 500; color: #fff; margin-left: 6px; }");
        sb.AppendLine(".description { font-style: italic; color: #757575; margin-left: 8px; font-size: 0.9em; }");

        AppendTypeStyles(sb);

        sb.AppendLine("</style>");
    }

    private void AppendTypeStyles(StringBuilder sb)
    {
        var types = new[]
        {
            MongoDB.Bson.BsonType.String,
            MongoDB.Bson.BsonType.Int32,
            MongoDB.Bson.BsonType.Int64,
            MongoDB.Bson.BsonType.Double,
            MongoDB.Bson.BsonType.Decimal128,
            MongoDB.Bson.BsonType.Boolean,
            MongoDB.Bson.BsonType.DateTime,
            MongoDB.Bson.BsonType.ObjectId,
            MongoDB.Bson.BsonType.Document,
            MongoDB.Bson.BsonType.Array,
            MongoDB.Bson.BsonType.Null
        };

        foreach (var type in types)
        {
            var cssClass = _colorProvider.GetCssClass(type);
            var color = _colorProvider.GetColor(type);
            sb.AppendLine($".{cssClass} {{ background-color: {color}; }}");
        }
    }

    private static void AppendStatistics(StringBuilder sb, SchemaStats stats)
    {
        sb.AppendLine("<div class=\"stats\">");
        AppendStatItem(sb, stats.DatabaseCount.ToString(), "Databases");
        AppendStatItem(sb, stats.CollectionCount.ToString(), "Collections");
        AppendStatItem(sb, stats.FieldCount.ToString(), "Fields");
        AppendStatItem(sb, $"{stats.CoveragePercent}%", "Coverage");
        sb.AppendLine("</div>");
    }

    private static void AppendStatItem(StringBuilder sb, string value, string label)
    {
        sb.AppendLine("<div class=\"stat-item\">");
        sb.AppendLine($"<div class=\"stat-value\">{WebUtility.HtmlEncode(value)}</div>");
        sb.AppendLine($"<div class=\"stat-label\">{WebUtility.HtmlEncode(label)}</div>");
        sb.AppendLine("</div>");
    }

    private void AppendSchemas(StringBuilder sb, List<DatabaseSchema> schemas)
    {
        foreach (var schema in schemas)
        {
            sb.AppendLine("<details class=\"database\" open>");
            sb.AppendLine($"<summary>{WebUtility.HtmlEncode(schema.DatabaseName)}</summary>");

            if (!string.IsNullOrEmpty(schema.Description))
            {
                sb.AppendLine($"<span class=\"description\">{WebUtility.HtmlEncode(schema.Description)}</span>");
            }

            foreach (var collection in schema.Collections)
            {
                AppendNode(sb, collection, isCollection: true);
            }

            sb.AppendLine("</details>");
        }
    }

    private void AppendNode(StringBuilder sb, DocumentNode node, bool isCollection = false)
    {
        if (node.ChildNodes.Count > 0)
        {
            var cssClass = isCollection ? "collection" : "";
            sb.AppendLine($"<details class=\"{cssClass}\">");
            sb.Append($"<summary>{WebUtility.HtmlEncode(node.FieldName)}");
            AppendTypeBadge(sb, node.FieldType);

            if (!string.IsNullOrEmpty(node.Description))
            {
                sb.Append($"<span class=\"description\">{WebUtility.HtmlEncode(node.Description)}</span>");
            }

            sb.AppendLine("</summary>");

            foreach (var child in node.ChildNodes)
            {
                AppendNode(sb, child);
            }

            sb.AppendLine("</details>");
        }
        else
        {
            sb.Append($"<div class=\"field\">{WebUtility.HtmlEncode(node.FieldName)}");
            AppendTypeBadge(sb, node.FieldType);

            if (!string.IsNullOrEmpty(node.Description))
            {
                sb.Append($"<span class=\"description\">{WebUtility.HtmlEncode(node.Description)}</span>");
            }

            sb.AppendLine("</div>");
        }
    }

    private void AppendTypeBadge(StringBuilder sb, MongoDB.Bson.BsonType bsonType)
    {
        var cssClass = _colorProvider.GetCssClass(bsonType);
        sb.Append($"<span class=\"type-badge {cssClass}\">{WebUtility.HtmlEncode(bsonType.ToString())}</span>");
    }

    private static SchemaStats CalculateStatistics(DatabaseDocumentation documentation)
    {
        var databaseCount = documentation.Schemas.Count;
        var collectionCount = 0;
        var fieldCount = 0;
        var describedCount = 0;

        foreach (var schema in documentation.Schemas)
        {
            collectionCount += schema.Collections.Count;
            foreach (var collection in schema.Collections)
            {
                CountFields(collection, ref fieldCount, ref describedCount);
            }
        }

        var coverage = fieldCount == 0 ? 0 : (int)(describedCount / (double)fieldCount * 100);
        return new SchemaStats(databaseCount, collectionCount, fieldCount, coverage);
    }

    private static void CountFields(DocumentNode node, ref int fieldCount, ref int describedCount)
    {
        fieldCount++;
        if (!string.IsNullOrEmpty(node.Description))
        {
            describedCount++;
        }

        foreach (var child in node.ChildNodes)
        {
            CountFields(child, ref fieldCount, ref describedCount);
        }
    }

    private record SchemaStats(int DatabaseCount, int CollectionCount, int FieldCount, int CoveragePercent);
}
