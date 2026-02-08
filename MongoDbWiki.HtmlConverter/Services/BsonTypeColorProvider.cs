using MongoDB.Bson;

namespace MongoDbWiki.HtmlConverter.Services;

public class BsonTypeColorProvider : IBsonTypeColorProvider
{
    public string GetCssClass(BsonType bsonType)
    {
        return $"type-{bsonType.ToString().ToLowerInvariant()}";
    }

    public string GetColor(BsonType bsonType)
    {
        return bsonType switch
        {
            BsonType.String => "#2e7d32",
            BsonType.Int32 => "#1565c0",
            BsonType.Int64 => "#1565c0",
            BsonType.Double => "#1565c0",
            BsonType.Decimal128 => "#1565c0",
            BsonType.Boolean => "#7b1fa2",
            BsonType.DateTime => "#e65100",
            BsonType.ObjectId => "#795548",
            BsonType.Document => "#455a64",
            BsonType.Array => "#00695c",
            BsonType.Null => "#9e9e9e",
            _ => "#616161"
        };
    }
}
