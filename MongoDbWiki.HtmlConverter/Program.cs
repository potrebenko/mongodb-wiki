using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDbWiki.HtmlConverter.Configuration;
using MongoDbWiki.HtmlConverter.Services;

namespace MongoDbWiki.HtmlConverter;

class Program
{
    public static readonly Dictionary<string, string> SwitchMappings = new()
    {
        { "--input", "HtmlConverter:InputJsonFile" },
        { "-i", "HtmlConverter:InputJsonFile" },
        { "--output", "HtmlConverter:OutputHtmlFile" },
        { "-o", "HtmlConverter:OutputHtmlFile" },
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
                services.Configure<HtmlConverterOptions>(context.Configuration.GetSection(HtmlConverterOptions.SectionName));
                services.AddSingleton<IJsonDocumentationReader, JsonDocumentationReader>();
                services.AddSingleton<IBsonTypeColorProvider, BsonTypeColorProvider>();
                services.AddSingleton<IHtmlGenerator, HtmlGenerator>();
                services.AddHostedService<HtmlConverterBackgroundService>();
            }).Build();

        host.Run();
    }
}
