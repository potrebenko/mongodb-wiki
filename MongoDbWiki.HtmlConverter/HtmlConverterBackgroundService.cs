using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDbWiki.HtmlConverter.Configuration;
using MongoDbWiki.HtmlConverter.Services;

namespace MongoDbWiki.HtmlConverter;

public class HtmlConverterBackgroundService : BackgroundService
{
    private readonly IJsonDocumentationReader _reader;
    private readonly IHtmlGenerator _htmlGenerator;
    private readonly HtmlConverterOptions _options;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public HtmlConverterBackgroundService(
        IJsonDocumentationReader reader,
        IHtmlGenerator htmlGenerator,
        IOptions<HtmlConverterOptions> options,
        IHostApplicationLifetime applicationLifetime)
    {
        _reader = reader;
        _htmlGenerator = htmlGenerator;
        _options = options.Value;
        _applicationLifetime = applicationLifetime;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine($"Reading JSON from {_options.InputJsonFile}...");
            var documentation = _reader.ReadFromFile(_options.InputJsonFile);

            Console.WriteLine("Generating HTML...");
            var html = _htmlGenerator.GenerateHtml(documentation, _options.PageTitle);

            File.WriteAllText(_options.OutputHtmlFile, html);
            Console.WriteLine($"Successfully wrote HTML to {_options.OutputHtmlFile}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }

        _applicationLifetime.StopApplication();
        return Task.CompletedTask;
    }
}
