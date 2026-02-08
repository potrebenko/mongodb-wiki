using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MongoDbWiki.HtmlConverter.Configuration;

namespace MongoDbWiki.HtmlConverter.Tests;

public class CommandLineArgumentsTests
{
    private static IConfiguration BuildConfiguration(params string[] args)
    {
        return new ConfigurationBuilder()
            .AddCommandLine(args, Program.SwitchMappings)
            .Build();
    }

    [Fact]
    public void ShortFlag_I_ShouldMapToInputJsonFile()
    {
        var config = BuildConfiguration("-i", "schema.json");

        var options = new HtmlConverterOptions();
        config.GetSection(HtmlConverterOptions.SectionName).Bind(options);

        options.InputJsonFile.Should().Be("schema.json");
    }

    [Fact]
    public void LongFlag_Input_ShouldMapToInputJsonFile()
    {
        var config = BuildConfiguration("--input", "schema.json");

        var options = new HtmlConverterOptions();
        config.GetSection(HtmlConverterOptions.SectionName).Bind(options);

        options.InputJsonFile.Should().Be("schema.json");
    }

    [Fact]
    public void ShortFlag_O_ShouldMapToOutputHtmlFile()
    {
        var config = BuildConfiguration("-o", "docs.html");

        var options = new HtmlConverterOptions();
        config.GetSection(HtmlConverterOptions.SectionName).Bind(options);

        options.OutputHtmlFile.Should().Be("docs.html");
    }

    [Fact]
    public void LongFlag_Output_ShouldMapToOutputHtmlFile()
    {
        var config = BuildConfiguration("--output", "docs.html");

        var options = new HtmlConverterOptions();
        config.GetSection(HtmlConverterOptions.SectionName).Bind(options);

        options.OutputHtmlFile.Should().Be("docs.html");
    }

    [Fact]
    public void MultipleFlags_ShouldMapAllValues()
    {
        var config = BuildConfiguration("-i", "my.json", "-o", "my.html");

        var options = new HtmlConverterOptions();
        config.GetSection(HtmlConverterOptions.SectionName).Bind(options);

        options.InputJsonFile.Should().Be("my.json");
        options.OutputHtmlFile.Should().Be("my.html");
    }

    [Fact]
    public void NoFlags_ShouldKeepDefaults()
    {
        var config = BuildConfiguration();

        var options = new HtmlConverterOptions();
        config.GetSection(HtmlConverterOptions.SectionName).Bind(options);

        options.InputJsonFile.Should().Be("output.json");
        options.OutputHtmlFile.Should().Be("schema.html");
        options.PageTitle.Should().Be("MongoDB Schema Documentation");
    }
}
