# Viae.Fluid.Markdown.Mvc

ASP.NET MVC integration library for [Viae.Fluid.Markdown](../Viae.Fluid.Markdown), enabling seamless Markdown rendering in Fluid MVC views with proper HTML encoding handling.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0%2B-blue.svg)](https://dotnet.microsoft.com/)

## Features

- **One-Line Setup** - Single extension method configures everything
- **No Double-Encoding** - Properly handles HTML content in Razor views
- **Fluent Configuration** - Chain configuration with other Fluid MVC options
- **Full MVC Integration** - Works seamlessly with Fluid.MvcViewEngine
- **Inherits Core Features** - All Markdown features from Viae.Fluid.Markdown
- **Fully Documented** - Complete XML documentation for IntelliSense

## What's Different from Viae.Fluid.Markdown?

This library extends `Viae.Fluid.Markdown` with MVC-specific functionality:

| Feature | Viae.Fluid.Markdown | Viae.Fluid.Markdown.Mvc |
|---------|---------------------|-------------------------|
| Target Framework | .NET Standard 2.0 | .NET 8.0+ |
| Use Case | Console apps, APIs, non-MVC | ASP.NET MVC with Razor views |
| Output Type | Plain string | `HtmlString` / `IHtmlContent` |
| Double-Encoding Prevention | No | Yes |
| Integration Method | Manual setup | Fluent extension method |
| Dependencies | Fluid.Core | Fluid.MvcViewEngine |

**When to use this library**: If you're building an ASP.NET MVC application with Fluid views and need to render Markdown content in Razor views without double-encoding issues.

**When to use the core library**: For console apps, APIs, or non-MVC scenarios where you need direct Markdown-to-HTML conversion.

## Installation

```bash
dotnet add package Viae.Fluid.Markdown.Mvc
```

This package automatically includes `Viae.Fluid.Markdown` as a dependency.

## Quick Start

### Basic Setup

In your `Program.cs`:

```csharp
using Viae.Fluid.Markdown.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add Fluid MVC with Markdown support
builder.Services.AddFluid(options =>
{
    // Add Markdown filters with default configuration (GitHub Flavored Markdown)
    options.AddFluidMarkdownFilters();
});

var app = builder.Build();
app.Run();
```

### In Your Views

Create a Fluid view (e.g., `Views/Post/Show.liquid`):

```liquid
<article>
    <h1>{{ post.title }}</h1>
    <div class="content">
        {{ post.content | markdown }}
    </div>
</article>
```

The Markdown will be rendered as HTML without double-encoding issues.

### Custom Configuration

Configure the Markdig pipeline with specific extensions:

```csharp
builder.Services.AddFluid(options =>
{
    options.AddFluidMarkdownFilters(opts =>
    {
        opts.ConfigurePipeline = builder => builder
            .UseAdvancedExtensions()      // GitHub Flavored Markdown
            .UseEmphasisExtras()           // Extra emphasis features
            .UseEmojiAndSmiley()          // Emoji support
            .UseDiagrams();                // Mermaid diagrams
    });
});
```

## How It Works

### The Double-Encoding Problem

When rendering Markdown in ASP.NET MVC Razor views, you can encounter double-encoding:

```html
<!-- Without proper handling: -->
&lt;p&gt;&lt;strong&gt;Bold&lt;/strong&gt;&lt;/p&gt;

<!-- With Viae.Fluid.Markdown.Mvc: -->
<p><strong>Bold</strong></p>
```

### The Solution

This library solves the problem through three components:

1. **`HtmlContentFluidValue`**: Wraps `IHtmlContent` to preserve HTML encoding semantics in Fluid
2. **Value Converter**: Automatically converts `IHtmlContent` values to `HtmlContentFluidValue`
3. **MVC Adapter**: Returns Markdown output as `HtmlString` instead of plain strings

```csharp
// What AddFluidMarkdownFilters() does internally:

// 1. Register value converter for IHtmlContent
options.TemplateOptions.ValueConverters.Add(x =>
    x is IHtmlContent h ? new HtmlContentFluidValue(h) : null
);

// 2. Build Markdown pipeline
var pipeline = opts.ConfigurePipeline(new MarkdownPipelineBuilder()).Build();

// 3. Create renderer and filter
var renderer = new MarkdigMarkdownRenderer(pipeline);
var core = new MarkdownFilter(renderer);

// 4. Wrap in MVC adapter and register
var adapter = new MarkdownMvcAdapter(core);
adapter.Register(options.TemplateOptions);
```

## API Reference

### FluidMarkdownExtensions.AddFluidMarkdownFilters

```csharp
public static FluidViewEngineOptions AddFluidMarkdownFilters(
    this FluidViewEngineOptions options,
    Action<MarkdownFilterOptions>? configure = null
)
```

Adds Markdown filters to Fluid MVC view engine with proper HTML encoding handling.

**Parameters:**
- `options` - The `FluidViewEngineOptions` to configure
- `configure` - Optional configuration action for customizing the Markdown pipeline

**Returns:** The configured `FluidViewEngineOptions` for fluent chaining

**Example:**
```csharp
services.AddFluid(options =>
{
    options.AddFluidMarkdownFilters(opts =>
    {
        opts.ConfigurePipeline = b => b.UseAdvancedExtensions();
    });
});
```

### HtmlContentFluidValue

A Fluid value wrapper for `IHtmlContent` that preserves HTML encoding semantics.

This class is used internally and you typically don't need to interact with it directly. It's automatically registered as a value converter when you call `AddFluidMarkdownFilters()`.

## Supported Markdown Features

All features from [Viae.Fluid.Markdown](../Viae.Fluid.Markdown#supported-markdown-features) are supported:

- Basic Markdown: Bold, italic, headings, links, images, code blocks
- Tables: Pipe tables and grid tables
- Task Lists: `- [ ]` and `- [x]` checkboxes
- Strikethrough: `~~text~~`
- Auto-identifiers for headings
- Footnotes
- Definition lists
- Abbreviations
- And more via Markdig's advanced extensions

## Filter Names

Both filters are registered with two names for compatibility:

- `markdown` - Standard filter name
- `markdownify` - Jekyll/Liquid compatible alias

```liquid
<!-- Both work identically: -->
{{ content | markdown }}
{{ content | markdownify }}
```

## Examples

### Blog Post View

```liquid
<!-- Views/Blog/Post.liquid -->
<!DOCTYPE html>
<html>
<head>
    <title>{{ post.title }}</title>
</head>
<body>
    <article>
        <header>
            <h1>{{ post.title }}</h1>
            <time>{{ post.publishedAt | date: "%B %d, %Y" }}</time>
        </header>

        <div class="excerpt">
            {{ post.excerpt | markdown }}
        </div>

        <div class="content">
            {{ post.content | markdown }}
        </div>

        <footer>
            <p>Tags: {{ post.tags | join: ", " }}</p>
        </footer>
    </article>
</body>
</html>
```

### Documentation Page

```liquid
<!-- Views/Docs/Page.liquid -->
<div class="documentation">
    <aside class="sidebar">
        {{ page.toc | markdown }}
    </aside>

    <main>
        {{ page.content | markdown }}
    </main>
</div>
```

### Comment System

```liquid
<!-- Views/Shared/_Comment.liquid -->
<div class="comment">
    <div class="comment-author">{{ comment.author }}</div>
    <div class="comment-body">
        {{ comment.body | markdown }}
    </div>
    <div class="comment-meta">{{ comment.createdAt | date: "%B %d, %Y" }}</div>
</div>
```

### Custom Pipeline Example

```csharp
// In Program.cs - Enable diagrams and emoji
builder.Services.AddFluid(options =>
{
    options.AddFluidMarkdownFilters(opts =>
    {
        opts.ConfigurePipeline = pipeline => pipeline
            .UseAdvancedExtensions()
            .UseEmojiAndSmiley()
            .UseDiagrams()
            .UseYamlFrontMatter();
    });
});
```

Then in views:
```liquid
{{ post.content | markdown }}
<!-- Supports emoji: :smile: becomes 😄 -->
<!-- Supports diagrams: Mermaid syntax -->
```

## Architecture

### Component Relationships

```
Viae.Fluid.Markdown (core library)
├── IMarkdownRenderer
├── MarkdigMarkdownRenderer
├── MarkdownFilter
├── MarkdownMvcAdapter
└── MarkdownFilterOptions

Viae.Fluid.Markdown.Mvc (this library)
├── FluidMarkdownExtensions ──> Uses all components from core
└── HtmlContentFluidValue    ──> Wraps IHtmlContent for Fluid
```

### Integration Flow

```
1. Application Startup
   └─> AddFluidMarkdownFilters() called
       ├─> Registers HtmlContentFluidValue converter
       ├─> Builds Markdig pipeline
       ├─> Creates MarkdownMvcAdapter
       └─> Registers filters with TemplateOptions

2. View Rendering
   └─> {{ content | markdown }}
       ├─> MarkdownMvcAdapter.InvokeAsync()
       ├─> MarkdownFilter renders to HTML string
       ├─> Wraps in HtmlString
       ├─> Converts to HtmlContentFluidValue
       └─> Writes to output without encoding
```

## Dependencies

- [Fluid.MvcViewEngine](https://github.com/sebastienros/fluid) (^2.30.0) - Liquid template engine for ASP.NET MVC
- [Viae.Fluid.Markdown](../Viae.Fluid.Markdown) - Core Markdown rendering library
- [Markdig](https://github.com/xoofx/markdig) (^0.43.0) - Markdown processor (transitive)
- [Microsoft.AspNetCore.Html.Abstractions](https://www.nuget.org/packages/Microsoft.AspNetCore.Html.Abstractions/) - For IHtmlContent (transitive)

## Target Framework

- .NET 8.0+
- .NET 10.0+

## Testing

This library includes comprehensive test coverage with 46+ unit tests using MSTest and FluentAssertions:

```bash
cd tests/Viae.Fluid.Markdown.Mvc.Tests
dotnet test
```

**Test Coverage:**
- **HtmlContentFluidValueTests** (24 tests) - Tests for the IHtmlContent wrapper
  - Constructor and null handling
  - Type system conversions (boolean, number, string)
  - Equality comparisons
  - WriteToAsync and HTML encoding preservation
  - Custom IHtmlContent implementations

- **FluidMarkdownExtensionsTests** (22 tests) - Tests for the extension method
  - Filter registration (markdown, markdownify)
  - Value converter registration
  - Markdown rendering (basic and complex)
  - Custom pipeline configuration
  - All Markdown features (tables, task lists, links, code, etc.)
  - HTML encoding preservation (no double-encoding)
  - Null and empty input handling

All tests pass with zero warnings and follow DRY principles with shared test helpers.

## Troubleshooting

### Markdown is displayed as text instead of HTML

**Problem**: You see `<p><strong>text</strong></p>` in your browser instead of rendered HTML.

**Solution**: Make sure you're not using `| escape` or other encoding filters after `| markdown`:

```liquid
<!-- Wrong: -->
{{ content | markdown | escape }}

<!-- Correct: -->
{{ content | markdown }}
```

### HTML is double-encoded in views

**Problem**: You see `&lt;p&gt;` instead of `<p>` tags.

**Solution**: Ensure you're using `Viae.Fluid.Markdown.Mvc` (not just `Viae.Fluid.Markdown`) and called `AddFluidMarkdownFilters()`:

```csharp
// Correct:
options.AddFluidMarkdownFilters();
```

### Markdown features not working

**Problem**: Tables, task lists, or other features don't render.

**Solution**: Configure the pipeline with the extensions you need:

```csharp
options.AddFluidMarkdownFilters(opts =>
{
    opts.ConfigurePipeline = b => b.UseAdvancedExtensions(); // Enables most features
});
```

## Contributing

This library is part of the [Viae](https://github.com/curt/viae) project. Contributions are welcome!

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Credits

- Built with [Fluid](https://github.com/sebastienros/fluid) by Sébastien Ros
- Markdown processing by [Markdig](https://github.com/xoofx/markdig) by Alexandre Mutel
- Core library: [Viae.Fluid.Markdown](../Viae.Fluid.Markdown)

## Related Projects

- [Viae.Fluid.Markdown](../Viae.Fluid.Markdown) - Core Markdown rendering library
- [Fluid](https://github.com/sebastienros/fluid) - .NET Liquid template engine
- [Markdig](https://github.com/xoofx/markdig) - Fast, powerful Markdown processor
- [Viae](https://github.com/curt/viae) - Parent project

## Support

For questions, issues, or feature requests:
- Create an issue in the [Viae repository](https://github.com/curt/viae/issues)
- Refer to the XML documentation in your IDE
- Check the [Fluid documentation](https://github.com/sebastienros/fluid)
- Check the [Markdig documentation](https://github.com/xoofx/markdig)
- See [Viae.Fluid.Markdown README](../Viae.Fluid.Markdown/README.md) for core features
