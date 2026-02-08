using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace MongoDbWiki.Core.Converters;

public class BsonTypeJsonConverter : JsonConverter<BsonType>
{
    public override BsonType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Enum.Parse<BsonType>(value!);
    }

    public override void Write(Utf8JsonWriter writer, BsonType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}