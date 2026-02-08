using Microsoft.Extensions.Options;
using MongoDbWiki.Core.Configuration;

namespace MongoDbWiki.Core.Services;

public class FileWriter : IWriter<string>
{
    private readonly string _outputPath;

    public FileWriter(IOptions<OutputOptions> options)
    {
        _outputPath = options.Value.OutputFile;
    }

    public void Write(string output)
    {
        try
        {
            File.WriteAllText(_outputPath, output);
            Console.WriteLine($"Successfully wrote output to {_outputPath}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error writing output to {_outputPath}: {e.Message}");
        }
    }
}