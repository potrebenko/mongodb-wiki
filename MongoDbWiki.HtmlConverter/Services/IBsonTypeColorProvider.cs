using MongoDB.Bson;

namespace MongoDbWiki.HtmlConverter.Services;

public interface IBsonTypeColorProvider
{
    string GetCssClass(BsonType bsonType);
    string GetColor(BsonType bsonType);
}
