using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDbWiki.Core.Configuration;
using MongoDbWiki.Core.Services;

namespace MongoDatabaseWiki;

class Program
{
    public static readonly Dictionary<string, string> SwitchMappings = new()
    {
        { "--output", "Output:OutputFile" },
        { "-o", "Output:OutputFile" },
        { "--merge-with", "Output:MergeWithFile" },
        { "-m", "Output:MergeWithFile" },
        { "--max-docs", "ConnectionSettings:MaximumDocumentsToExamine" },
    };

    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args, SwitchMappings);
            })
            .ConfigureLogging(logging => logging.ClearProviders())
            .ConfigureServices((context, services) =>
            {
                services.Configure<ConnectionOptions>(context.Configuration.GetSection(ConnectionOptions.SectionName));
                services.Configure<OutputOptions>(context.Configuration.GetSection(OutputOptions.SectionName));
                services.AddSingleton<IMongoDbProviderFactory, MongoDbProviderFactory>();
                services.AddSingleton<IMongoDbSchemaScanner, MongoDbSchemaScanner>();
                services.AddSingleton<IOutputBuilder, JsonOutputBuilder>();
                services.AddSingleton<IDocumentationStatisticsProvider, DocumentationStatisticsProvider>();
                services.AddSingleton<ISchemaMerger, SchemaMerger>();
                services.AddSingleton<IWriter<string>, FileWriter>();
                services.AddSingleton<IStatisticPrinter, StatisticPrinter>();
                services.AddHostedService<ConsoleBackgroundService>();
            }).Build();

        host.Run();
    }
}