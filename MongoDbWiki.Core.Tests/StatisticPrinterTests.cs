using FluentAssertions;
using MongoDbWiki.Core.Models;
using MongoDbWiki.Core.Services;

namespace MongoDbWiki.Core.Tests;

public class StatisticPrinterTests
{
    private readonly StatisticPrinter _sut = new();

    [Fact]
    public void Print_ShouldOutputStatisticsToConsole()
    {
        // Arrange
        var statistics = new DocumentationStatistics
        {
            OldVersion = "1",
            TotalCollectionsOldSchema = 3,
            TotalFieldsInOldSchema = 15,
            TotalCollectionsNewSchema = 4,
            TotalFieldsInNewSchema = 20,
            TotalItemsWithDescription = 10
        };

        using var writer = new StringWriter();
        Console.SetOut(writer);

        // Act
        _sut.Print(statistics);

        // Assert
        var output = writer.ToString();
        output.Should().Contain("DB Statistics");
        output.Should().Contain("Collections: 3");
        output.Should().Contain("Fields: 15");
        output.Should().Contain("Collections: 4");
        output.Should().Contain("Fields: 20");
        output.Should().Contain("Coverage: 50%");

        // Reset console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
    }
}