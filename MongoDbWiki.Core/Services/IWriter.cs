namespace MongoDbWiki.Core.Services;

public interface IWriter<in T>
{
    void Write(T output);
}