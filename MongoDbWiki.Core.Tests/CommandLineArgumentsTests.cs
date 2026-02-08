using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MongoDbWiki.Core.Configuration;
using MongoDatabaseWiki;

namespace MongoDbWiki.Core.Tests;

public class CommandLineArgumentsTests
{
    private static IConfiguration BuildConfiguration(params string[] args)
    {
        return new ConfigurationBuilder()
            .AddCommandLine(args, Program.SwitchMappings)
            .Build();
    }

    [Fact]
    public void ShortFlag_O_ShouldMapToOutputFile()
    {
        var config = BuildConfiguration("-o", "result.json");

        var options = new OutputOptions();
        config.GetSection(OutputOptions.SectionName).Bind(options);

        options.OutputFile.Should().Be("result.json");
    }

    [Fact]
    public void LongFlag_Output_ShouldMapToOutputFile()
    {
        var config = BuildConfiguration("--output", "result.json");

        var options = new OutputOptions();
        config.GetSection(OutputOptions.SectionName).Bind(options);

        options.OutputFile.Should().Be("result.json");
    }

    [Fact]
    public void ShortFlag_M_ShouldMapToMergeWithFile()
    {
        var config = BuildConfiguration("-m", "old.json");

        var options = new OutputOptions();
        config.GetSection(OutputOptions.SectionName).Bind(options);

        options.MergeWithFile.Should().Be("old.json");
    }

    [Fact]
    public void LongFlag_MergeWith_ShouldMapToMergeWithFile()
    {
        var config = BuildConfiguration("--merge-with", "old.json");

        var options = new OutputOptions();
        config.GetSection(OutputOptions.SectionName).Bind(options);

        options.MergeWithFile.Should().Be("old.json");
    }

    [Fact]
    public void LongFlag_MaxDocs_ShouldMapToMaximumDocumentsToExamine()
    {
        var config = BuildConfiguration("--max-docs", "25");

        var options = new ConnectionOptions();
        config.GetSection(ConnectionOptions.SectionName).Bind(options);

        options.MaximumDocumentsToExamine.Should().Be(25);
    }

    [Fact]
    public void MultipleFlags_ShouldMapAllValues()
    {
        var config = BuildConfiguration("-o", "out.json", "-m", "prev.json", "--max-docs", "50");

        var outputOptions = new OutputOptions();
        config.GetSection(OutputOptions.SectionName).Bind(outputOptions);

        var connectionOptions = new ConnectionOptions();
        config.GetSection(ConnectionOptions.SectionName).Bind(connectionOptions);

        outputOptions.OutputFile.Should().Be("out.json");
        outputOptions.MergeWithFile.Should().Be("prev.json");
        connectionOptions.MaximumDocumentsToExamine.Should().Be(50);
    }
}
