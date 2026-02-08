using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace MongoDbWiki.Core.Models;

public class DocumentNode
{
    public string FieldNamespace { get; private set; }
    public string FieldName { get; private set; }
    public BsonType FieldType { get; private set; }
    public string Description { get; private set; }
    public List<DocumentNode> ChildNodes { get; private set; }

    [JsonConstructor]
    public DocumentNode(
        string fieldNamespace,
        string fieldName,
        BsonType fieldType,
        string description,
        List<DocumentNode> childNodes)
    {
        FieldNamespace = fieldNamespace;
        FieldName = fieldName;
        FieldType = fieldType;
        ChildNodes = childNodes;
        Description = description;
    }

    public DocumentNode(
        string fieldNamespace, 
        string fieldName, 
        BsonType fieldType) : 
        this(fieldNamespace, fieldName, fieldType, string.Empty, new List<DocumentNode>())
    {
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void AddNode(DocumentNode node)
    {
        ChildNodes.Add(node);
    }
}