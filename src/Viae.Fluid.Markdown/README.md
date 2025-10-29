# Viae.Fluid.Markdown

A .NET library that provides Markdown rendering support for [Fluid](https://github.com/sebastienros/fluid) template engine, with special support for ASP.NET MVC applications.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-Standard%202.0-blue.svg)](https://dotnet.microsoft.com/)

## Features

- 🚀 **Easy Integration** - Simple setup with Fluid templates
- 📝 **GitHub Flavored Markdown** - Full support via Markdig's advanced extensions
- 🎨 **MVC Support** - Special adapter for ASP.NET MVC to prevent double-encoding
- ⚙️ **Customizable** - Configure Markdig pipeline extensions as needed
- 🧪 **Well Tested** - Comprehensive test coverage with 60+ unit tests
- 📖 **Fully Documented** - Complete XML documentation for IntelliSense

## Supported Markdown Features

With the default configuration, this library supports:

- **Basic Markdown**: Bold, italic, headings, links, images, code blocks
- **Tables**: Pipe tables and grid tables
- **Task Lists**: `- [ ]` and `- [x]` checkboxes
- **Strikethrough**: `~~text~~`
- **Auto-identifiers**: Automatic heading IDs
- **Footnotes**: `[^1]` style footnotes
- **Definition Lists**: Term and definition syntax
- **Abbreviations**: Automatic abbreviation expansion
- And more via Markdig's advanced extensions

## Installation

```bash
dotnet add package Viae.Fluid.Markdown
```

## Quick Start

### Basic Usage (Non-MVC)

```csharp
using Viae.Fluid.Markdown;

// Create the filter and template options
var (filter, options) = FluidMarkdownCoreRegistration.CreateCore();

// Create a Fluid template context
var context = new TemplateContext(options);
context.SetValue("content", "**Hello** from _Markdown_!");

// Parse and render the template
var parser = new FluidParser();
if (parser.TryParse("{{ content | markdown }}", out var template, out var error))
{
    var output = await template.RenderAsync(context);
    Console.WriteLine(output);
    // Output: <p><strong>Hello</strong> from <em>Markdown</em>!</p>
}
```

### ASP.NET MVC Usage

For MVC applications, use the `MarkdownMvcAdapter` to prevent HTML double-encoding:

```csharp
using Viae.Fluid.Markdown;

// In your Startup.cs or Program.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton(sp =>
    {
        var (coreFilter, _) = FluidMarkdownCoreRegistration.CreateCore();
        var mvcAdapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        mvcAdapter.Register(options);
        return options;
    });
}
```

Then in your Razor views with Fluid:

```html
@* The markdown filter will render HTML without double-encoding *@
@await RenderAsync("{{ post.content | markdown }}")
```

### Custom Configuration

Customize the Markdig pipeline to enable specific extensions:

```csharp
using Viae.Fluid.Markdown;
using Markdig;

var customOptions = new MarkdownFilterOptions
{
    ConfigurePipeline = builder => builder
        .UseAdvancedExtensions()      // GitHub Flavored Markdown
        .UseEmphasisExtras()           // Extra emphasis features
        .UseGridTables()               // Grid-style tables
        .UsePipeTables()               // Pipe-style tables
        .UseTaskLists()                // Task list checkboxes
        .UseAutoIdentifiers()          // Auto heading IDs
};

var (filter, templateOptions) = FluidMarkdownCoreRegistration.CreateCore(customOptions);
```

## API Overview

### Core Classes

#### `IMarkdownRenderer`
Interface for Markdown rendering implementations.

```csharp
public interface IMarkdownRenderer
{
    string ToHtml(string markdown);
}
```

#### `MarkdigMarkdownRenderer`
Markdig-based implementation of `IMarkdownRenderer`.

```csharp
var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
var renderer = new MarkdigMarkdownRenderer(pipeline);
var html = renderer.ToHtml("**bold text**");
```

#### `MarkdownFilter`
Fluid filter for Markdown rendering (returns plain strings).

```csharp
var filter = new MarkdownFilter(renderer);
var options = new TemplateOptions();
filter.Register(options);
// Registers filters: "markdown" and "markdownify"
```

#### `MarkdownMvcAdapter`
MVC-specific adapter that wraps output in `HtmlString`.

```csharp
var adapter = new MarkdownMvcAdapter(coreFilter);
var options = new TemplateOptions();
adapter.Register(options);
// Use in MVC views to prevent double-encoding
```

#### `MarkdownFilterOptions`
Configuration options for the Markdown pipeline.

```csharp
var options = new MarkdownFilterOptions
{
    ConfigurePipeline = builder => builder.UseAdvancedExtensions()
};
```

#### `FluidMarkdownCoreRegistration`
Factory class for easy setup.

```csharp
var (filter, options) = FluidMarkdownCoreRegistration.CreateCore();
```

## Filter Names

Both filters are registered under two names for compatibility:

- `markdown` - Standard filter name
- `markdownify` - Jekyll/Liquid compatible alias

Both can be used interchangeably:

```liquid
{{ content | markdown }}
{{ content | markdownify }}
```

## Architecture

### When to Use What

| Scenario | Use | Reason |
|----------|-----|--------|
| ASP.NET MVC with Razor | `MarkdownMvcAdapter` | Returns `HtmlString` to prevent double-encoding |
| Console apps, APIs, tests | `MarkdownFilter` | Returns plain strings |
| Custom rendering logic | `IMarkdownRenderer` | Direct access to rendering |
| Quick setup | `FluidMarkdownCoreRegistration.CreateCore()` | Factory method handles all setup |

### Class Hierarchy

```
IMarkdownRenderer (interface)
└── MarkdigMarkdownRenderer (Markdig implementation)
    └── MarkdownFilter (Fluid filter, returns string)
        └── MarkdownMvcAdapter (MVC adapter, returns HtmlString)
```

## Examples

### Example 1: Blog Post Rendering

```csharp
var (filter, options) = FluidMarkdownCoreRegistration.CreateCore();
var context = new TemplateContext(options);

var blogPost = @"
# My Blog Post

This is a **markdown** blog post with:

- Bullet points
- *Italic text*
- [Links](https://example.com)

## Code Example

```csharp
Console.WriteLine(""Hello World"");
```
";

context.SetValue("post", blogPost);
var template = FluidTemplate.Parse("{{ post | markdown }}");
var html = await template.RenderAsync(context);
```

### Example 2: Multiple Markdown Fields

```csharp
var context = new TemplateContext(options);
context.SetValue("title", "# Welcome");
context.SetValue("excerpt", "This is a **short** excerpt.");
context.SetValue("content", "Full article content here...");

var template = FluidTemplate.Parse(@"
<article>
    <header>{{ title | markdown }}</header>
    <aside>{{ excerpt | markdown }}</aside>
    <main>{{ content | markdown }}</main>
</article>
");

var html = await template.RenderAsync(context);
```

### Example 3: Custom Pipeline (Minimal Extensions)

```csharp
// Create a minimal Markdown renderer with only basic features
var minimalOptions = new MarkdownFilterOptions
{
    ConfigurePipeline = builder => builder
        .UseEmphasisExtras()  // Bold and italic only
        .UsePipeTables()      // Tables only
};

var (filter, options) = FluidMarkdownCoreRegistration.CreateCore(minimalOptions);
```

## Testing

The library includes comprehensive tests (60+ test cases) using MSTest and FluentAssertions:

```bash
cd tests/Viae.Fluid.Markdown.Tests
dotnet test
```

Test coverage includes:
- All Markdown features (bold, italic, links, tables, etc.)
- Null and empty string handling
- Custom pipeline configurations
- Filter registration
- MVC adapter HtmlString wrapping

## Dependencies

- [Fluid.Core](https://github.com/sebastienros/fluid) (^2.30.0) - Liquid template engine
- [Markdig](https://github.com/xoofx/markdig) (^0.43.0) - Markdown processor
- [Microsoft.AspNetCore.Html.Abstractions](https://www.nuget.org/packages/Microsoft.AspNetCore.Html.Abstractions/) (^2.3.0) - For MVC support

## Target Framework

- .NET Standard 2.0 (compatible with .NET Core 2.0+, .NET Framework 4.6.1+, .NET 5+)

## Contributing

This library is part of the [Viae](https://github.com/curt/viae) project. Contributions are welcome!

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Credits

- Built with [Fluid](https://github.com/sebastienros/fluid) by Sébastien Ros
- Markdown processing by [Markdig](https://github.com/xoofx/markdig) by Alexandre Mutel

## Related Projects

- [Fluid](https://github.com/sebastienros/fluid) - .NET Liquid template engine
- [Markdig](https://github.com/xoofx/markdig) - Fast, powerful Markdown processor
- [Viae](https://github.com/curt/viae) - Parent project

## Support

For questions, issues, or feature requests:
- Create an issue in the [Viae repository](https://github.com/curt/viae/issues)
- Refer to the XML documentation in your IDE
- Check the [Fluid documentation](https://github.com/sebastienros/fluid)
- Check the [Markdig documentation](https://github.com/xoofx/markdig)
