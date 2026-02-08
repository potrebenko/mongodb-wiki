# MongoDB Database Wiki

A tool that scans MongoDB databases and generates schema documentation. It examines sample documents from collections to build a hierarchical representation of the document structure, outputs a JSON schema file, and can convert it to an interactive HTML page.

## Projects

| Project | Description |
|---------|-------------|
| **MongoDbWiki.Core** | Core library with schema scanning logic, models, and converters |
| **MongoDbWiki.Console** | CLI tool that connects to MongoDB and generates a JSON schema file |
| **MongoDbWiki.HtmlConverter** | CLI tool that converts the JSON schema into a static HTML page with an interactive tree view |
| **MongoDbWiki.Core.Tests** | Unit tests for the Core library |
| **MongoDbWiki.HtmlConverter.Tests** | Unit tests for the HTML converter |

## How It Works

### Schema Scanner (`MongoDbWiki.Console`)

Connects to one or more MongoDB databases, samples documents from each collection (up to a configurable limit), and recursively inspects them to build a complete field-level schema. The output is a JSON file describing every database, collection, and field along with its BSON type. If a previous JSON file exists, descriptions are merged forward so manually added documentation is preserved across scans.

### HTML Converter (`MongoDbWiki.HtmlConverter`)

Reads the JSON schema file produced by the scanner and generates a single self-contained HTML page. The page features:

- Collapsible tree navigation using native HTML5 `<details>/<summary>` elements (no JavaScript required)
- Color-coded type badges for each BSON type (String, Int32, ObjectId, Document, Array, etc.)
- Inline field descriptions shown in italics when present
- A statistics summary showing database count, collection count, field count, and documentation coverage percentage

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- A running MongoDB instance (for the schema scanner)

## Quick Start

### Build

```bash
dotnet build MongoDbDatabaseWiki.slnx
```

### Run from source

```bash
# 1. Scan MongoDB and generate JSON schema
dotnet run --project MongoDbWiki.Console

# 2. Convert JSON schema to HTML
dotnet run --project MongoDbWiki.HtmlConverter
```

Pass arguments after `--`:

```bash
dotnet run --project MongoDbWiki.Console -- -o schema.json --max-docs 20
dotnet run --project MongoDbWiki.HtmlConverter -- -i schema.json -o docs.html
```

### Run tests

```bash
# Run all tests
dotnet test MongoDbDatabaseWiki.slnx

# Run a specific test
dotnet test MongoDbWiki.Core.Tests --filter "FullyQualifiedName~ParserDocument_ShouldReturnOnlyUniqueFields"
```

## Install as .NET Tools

Both console applications can be packaged and installed as .NET global tools, making them available as commands from any directory.

### Schema Scanner (`mongodbwiki`)

```bash
# Pack
dotnet pack MongoDbWiki.Console -o ./nupkg

# Install globally
dotnet tool install --global --add-source ./nupkg MongoDbWiki.Console

# Run from anywhere
mongodbwiki
mongodbwiki -o schema.json -m previous.json --max-docs 20
```

### HTML Converter (`mongodbwiki-html`)

```bash
# Pack
dotnet pack MongoDbWiki.HtmlConverter -o ./nupkg

# Install globally
dotnet tool install --global --add-source ./nupkg MongoDbWiki.HtmlConverter

# Run from anywhere
mongodbwiki-html
mongodbwiki-html -i schema.json -o docs.html
```

### Uninstall

```bash
dotnet tool uninstall --global MongoDbWiki.Console
dotnet tool uninstall --global MongoDbWiki.HtmlConverter
```

## Configuration

Both tools can be configured via an `appsettings.json` file placed in the working directory, environment variables, or command-line arguments. Command-line arguments take the highest priority.

### Schema Scanner

```json
{
  "ConnectionSettings": {
    "ConnectionStrings": [
      {
        "Name": "MyDatabase",
        "ConnectionString": "mongodb://localhost/MyDatabase"
      }
    ],
    "MaximumDocumentsToExamine": 10
  },
  "Output": {
    "MergeWithFile": "original.json",
    "OutputFile": "output.json"
  }
}
```

| Setting | Description | Default |
|---------|-------------|---------|
| `ConnectionSettings:ConnectionStrings` | List of MongoDB connection strings to scan | — |
| `ConnectionSettings:MaximumDocumentsToExamine` | Number of sample documents per collection | `10` |
| `Output:OutputFile` | Path to the generated JSON file | `output.json` |
| `Output:MergeWithFile` | Path to a previous JSON file to merge descriptions from | `original.json` |

**CLI shorthand flags:**

| Flag | Long form | Maps to |
|------|-----------|---------|
| `-o` | `--output` | `Output:OutputFile` |
| `-m` | `--merge-with` | `Output:MergeWithFile` |
| | `--max-docs` | `ConnectionSettings:MaximumDocumentsToExamine` |

```bash
mongodbwiki -o schema.json -m previous.json --max-docs 20
```

### HTML Converter

```json
{
  "HtmlConverter": {
    "InputJsonFile": "output.json",
    "OutputHtmlFile": "schema.html"
  }
}
```

| Setting | Description | Default |
|---------|-------------|---------|
| `HtmlConverter:InputJsonFile` | Path to the JSON schema file | `output.json` |
| `HtmlConverter:OutputHtmlFile` | Path to the generated HTML file | `schema.html` |

**CLI shorthand flags:**

| Flag | Long form | Maps to |
|------|-----------|---------|
| `-i` | `--input` | `HtmlConverter:InputJsonFile` |
| `-o` | `--output` | `HtmlConverter:OutputHtmlFile` |

```bash
mongodbwiki-html -i myschema.json -o docs.html
```

## Typical Workflow

```bash
# 1. Scan your MongoDB databases
mongodbwiki

# 2. (Optional) Edit output.json to add field descriptions
#    Descriptions are preserved when re-scanning with --merge-with

# 3. Re-scan and merge descriptions from the previous run
mongodbwiki -m output.json

# 4. Generate the HTML documentation
mongodbwiki-html

# 5. Open schema.html in a browser
open schema.html
```
