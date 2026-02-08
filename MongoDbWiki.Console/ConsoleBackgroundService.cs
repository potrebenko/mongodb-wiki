using System.Reflection;
using Microsoft.Extensions.Hosting;
using MongoDbWiki.Core.Services;

namespace MongoDatabaseWiki;

public class ConsoleBackgroundService : BackgroundService
{
    private readonly IMongoDbProviderFactory _mongoDbProviderFactory;
    private readonly IMongoDbSchemaScanner _mongoDbSchemaScanner;
    private readonly IOutputBuilder _outputBuilder;
    private readonly IWriter<string> _outputWriter;
    private readonly IStatisticPrinter _statisticPrinter;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private const string ApplicationName = "MongoDbWiki";

    public ConsoleBackgroundService(
        IMongoDbProviderFactory mongoDbProviderFactory,
        IMongoDbSchemaScanner mongoDbSchemaScanner,
        IOutputBuilder outputBuilder,
        IWriter<string> outputWriter,
        IStatisticPrinter statisticPrinter,
        IHostApplicationLifetime applicationLifetime)
    {
        _mongoDbProviderFactory = mongoDbProviderFactory;
        _mongoDbSchemaScanner = mongoDbSchemaScanner;
        _outputBuilder = outputBuilder;
        _outputWriter = outputWriter;
        _statisticPrinter = statisticPrinter;
        _applicationLifetime = applicationLifetime;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var name = ApplicationName;
        var version = assembly.GetName().Version;
        Console.WriteLine($"{name} v{version}");

        try
        {
            var connections = _mongoDbProviderFactory.CreateProviders();
            var schemas = _mongoDbSchemaScanner.ScanDatabaseSchema(connections);
            var (output, statistics) = _outputBuilder.BuildOutput(schemas);
            _outputWriter.Write(output);
            _statisticPrinter.Print(statistics);
        }
        catch (Exception e)
        {
            Console.WriteLine("Application finished with error: " + e.Message);
        }

        Console.WriteLine("Completed processing");
        _applicationLifetime.StopApplication();
        return Task.CompletedTask;
    }
}