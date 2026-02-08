using FluentAssertions;
using MongoDB.Bson;
using MongoDbWiki.Core.Models;
using MongoDbWiki.HtmlConverter.Services;

namespace MongoDbWiki.HtmlConverter.Tests;

public class HtmlGeneratorTests
{
    private readonly HtmlGenerator _sut;

    public HtmlGeneratorTests()
    {
        _sut = new HtmlGenerator(new BsonTypeColorProvider());
    }

    [Fact]
    public void GenerateHtml_ShouldContainPageTitle()
    {
        // Arrange
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>());

        // Act
        var html = _sut.GenerateHtml(doc, "My Schema");

        // Assert
        html.Should().Contain("<title>My Schema</title>");
        html.Should().Contain("<h1>My Schema</h1>");
    }

    [Fact]
    public void GenerateHtml_WhenDocumentationHasTitle_ShouldUseDocumentationTitle()
    {
        // Arrange
        var doc = new DatabaseDocumentation("1", "Custom Title From JSON", new List<DatabaseSchema>());

        // Act
        var html = _sut.GenerateHtml(doc, "Fallback Title");

        // Assert
        html.Should().Contain("<title>Custom Title From JSON</title>");
        html.Should().Contain("<h1>Custom Title From JSON</h1>");
        html.Should().NotContain("Fallback Title");
    }

    [Fact]
    public void GenerateHtml_ShouldContainDatabaseName()
    {
        // Arrange
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("ProductionDb", "", new List<DocumentNode>())
        });

        // Act
        var html = _sut.GenerateHtml(doc, "Schema");

        // Assert
        html.Should().Contain("ProductionDb");
    }

    [Fact]
    public void GenerateHtml_ShouldContainFieldNames()
    {
        // Arrange
        var field = new DocumentNode("users.email", "email", BsonType.String);
        var collection = new DocumentNode("users", "users", BsonType.Document, "", new List<DocumentNode> { field });
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", "", new List<DocumentNode> { collection })
        });

        // Act
        var html = _sut.GenerateHtml(doc, "Schema");

        // Assert
        html.Should().Contain("email");
    }

    [Fact]
    public void GenerateHtml_ShouldRenderNestedNodesAsDetails()
    {
        // Arrange
        var child = new DocumentNode("users.address.street", "street", BsonType.String);
        var address = new DocumentNode("users.address", "address", BsonType.Document, "", new List<DocumentNode> { child });
        var collection = new DocumentNode("users", "users", BsonType.Document, "", new List<DocumentNode> { address });
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", "", new List<DocumentNode> { collection })
        });

        // Act
        var html = _sut.GenerateHtml(doc, "Schema");

        // Assert
        html.Should().Contain("<details");
        html.Should().Contain("<summary>address");
        html.Should().Contain("<summary>users");
    }

    [Fact]
    public void GenerateHtml_ShouldRenderLeafNodesAsDivs()
    {
        // Arrange
        var leaf = new DocumentNode("users.name", "name", BsonType.String);
        var collection = new DocumentNode("users", "users", BsonType.Document, "", new List<DocumentNode> { leaf });
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", "", new List<DocumentNode> { collection })
        });

        // Act
        var html = _sut.GenerateHtml(doc, "Schema");

        // Assert
        html.Should().Contain("<div class=\"field\">name");
    }

    [Fact]
    public void GenerateHtml_ShouldRenderTypeBadgesWithCssClasses()
    {
        // Arrange
        var field = new DocumentNode("users.name", "name", BsonType.String);
        var collection = new DocumentNode("users", "users", BsonType.Document, "", new List<DocumentNode> { field });
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", "", new List<DocumentNode> { collection })
        });

        // Act
        var html = _sut.GenerateHtml(doc, "Schema");

        // Assert
        html.Should().Contain("type-badge type-string");
        html.Should().Contain(">String</span>");
    }

    [Fact]
    public void GenerateHtml_ShouldRenderDescriptionWhenPresent()
    {
        // Arrange
        var field = new DocumentNode("users.name", "name", BsonType.String, "The user's full name", new List<DocumentNode>());
        var collection = new DocumentNode("users", "users", BsonType.Document, "", new List<DocumentNode> { field });
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", "", new List<DocumentNode> { collection })
        });

        // Act
        var html = _sut.GenerateHtml(doc, "Schema");

        // Assert
        html.Should().Contain("class=\"description\"");
        html.Should().Contain("The user&#39;s full name");
    }

    [Fact]
    public void GenerateHtml_ShouldNotRenderDescriptionWhenEmpty()
    {
        // Arrange
        var field = new DocumentNode("users.name", "name", BsonType.String);
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", "", new List<DocumentNode> { new("users", "users", BsonType.Document, "", new List<DocumentNode> { field }) })
        });

        // Act
        var html = _sut.GenerateHtml(doc, "Schema");

        // Assert
        var fieldDivIndex = html.IndexOf("<div class=\"field\">name");
        var nextDivOrDetails = html.IndexOfAny(['<'], fieldDivIndex + "<div class=\"field\">name".Length);
        var fieldLine = html.Substring(fieldDivIndex, nextDivOrDetails - fieldDivIndex + 30);
        fieldLine.Should().NotContain("class=\"description\"");
    }

    [Fact]
    public void GenerateHtml_ShouldContainStatistics()
    {
        // Arrange
        var field = new DocumentNode("users.name", "name", BsonType.String, "desc", new List<DocumentNode>());
        var collection = new DocumentNode("users", "users", BsonType.Document, "", new List<DocumentNode> { field });
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", "", new List<DocumentNode> { collection })
        });

        // Act
        var html = _sut.GenerateHtml(doc, "Schema");

        // Assert
        html.Should().Contain("class=\"stats\"");
        html.Should().Contain("Databases");
        html.Should().Contain("Collections");
        html.Should().Contain("Fields");
        html.Should().Contain("Coverage");
    }

    [Fact]
    public void GenerateHtml_ShouldHtmlEncodeSpecialCharacters()
    {
        // Arrange
        var field = new DocumentNode("col.<script>", "<script>alert('xss')</script>", BsonType.String);
        var collection = new DocumentNode("col", "col", BsonType.Document, "", new List<DocumentNode> { field });
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>
        {
            new("db", "", new List<DocumentNode> { collection })
        });

        // Act
        var html = _sut.GenerateHtml(doc, "Schema");

        // Assert
        html.Should().NotContain("<script>alert");
        html.Should().Contain("&lt;script&gt;");
    }

    [Fact]
    public void GenerateHtml_ShouldBeValidHtmlDocument()
    {
        // Arrange
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>());

        // Act
        var html = _sut.GenerateHtml(doc, "Test");

        // Assert
        html.Should().Contain("<!DOCTYPE html>");
        html.Should().Contain("<html");
        html.Should().Contain("</html>");
        html.Should().Contain("<head>");
        html.Should().Contain("</head>");
        html.Should().Contain("<body>");
        html.Should().Contain("</body>");
    }

    [Fact]
    public void GenerateHtml_ShouldContainEmbeddedStyles()
    {
        // Arrange
        var doc = new DatabaseDocumentation("1", "", new List<DatabaseSchema>());

        // Act
        var html = _sut.GenerateHtml(doc, "Test");

        // Assert
        html.Should().Contain("<style>");
        html.Should().Contain("</style>");
        html.Should().Contain(".type-string");
        html.Should().Contain(".type-document");
    }
}
