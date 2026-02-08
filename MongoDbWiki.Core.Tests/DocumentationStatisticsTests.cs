using FluentAssertions;
using MongoDbWiki.Core.Models;

namespace MongoDbWiki.Core.Tests;

public class DocumentationStatisticsTests
{
    [Fact]
    public void CalculateCoverage_ShouldReturnPercentage()
    {
        // Arrange
        var statistics = new DocumentationStatistics
        {
            TotalItemsWithDescription = 5,
            TotalFieldsInNewSchema = 10
        };

        // Act
        var coverage = statistics.CalculateCoverage();

        // Assert
        coverage.Should().Be(50);
    }

    [Fact]
    public void CalculateCoverage_WhenAllFieldsHaveDescriptions_ShouldReturn100()
    {
        // Arrange
        var statistics = new DocumentationStatistics
        {
            TotalItemsWithDescription = 10,
            TotalFieldsInNewSchema = 10
        };

        // Act
        var coverage = statistics.CalculateCoverage();

        // Assert
        coverage.Should().Be(100);
    }

    [Fact]
    public void CalculateCoverage_WhenNoFieldsInNewSchema_ShouldReturnZero()
    {
        // Arrange
        var statistics = new DocumentationStatistics
        {
            TotalItemsWithDescription = 0,
            TotalFieldsInNewSchema = 0
        };

        // Act
        var coverage = statistics.CalculateCoverage();

        // Assert
        coverage.Should().Be(0);
    }

    [Fact]
    public void CalculateCoverage_WhenNoDescriptions_ShouldReturnZero()
    {
        // Arrange
        var statistics = new DocumentationStatistics
        {
            TotalItemsWithDescription = 0,
            TotalFieldsInNewSchema = 10
        };

        // Act
        var coverage = statistics.CalculateCoverage();

        // Assert
        coverage.Should().Be(0);
    }

    [Fact]
    public void CalculateCoverage_ShouldTruncateToInt()
    {
        // Arrange
        var statistics = new DocumentationStatistics
        {
            TotalItemsWithDescription = 1,
            TotalFieldsInNewSchema = 3
        };

        // Act
        var coverage = statistics.CalculateCoverage();

        // Assert
        coverage.Should().Be(33);
    }
}