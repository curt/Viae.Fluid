# Viae.Fluid

Markdown rendering helpers for the [Fluid](https://github.com/sebastienros/fluid) Liquid template engine, powered by [Markdig](https://github.com/xoofx/markdig) and tuned for both generic .NET apps and ASP.NET MVC scenarios.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build and Publish](https://github.com/curt/Viae.Fluid/actions/workflows/build-and-publish.yml/badge.svg)](https://github.com/curt/Viae.Fluid/actions/workflows/build-and-publish.yml)
[![Coverage Status](https://coveralls.io/repos/github/curt/Viae.Fluid/badge.svg?branch=dev)](https://coveralls.io/github/curt/Viae.Fluid?branch=dev)

## Packages

| Package | Description | Target Frameworks |
| --- | --- | --- |
| [`Viae.Fluid.Markdown`](src/Viae.Fluid.Markdown/README.md) | Core Markdown filter for Fluid templates, returning HTML strings. | `netstandard2.0` |
| [`Viae.Fluid.Markdown.Mvc`](src/Viae.Fluid.Markdown.Mvc/README.md) | MVC adapter that integrates the Markdown filter with Fluid.MvcViewEngine and prevents double-encoding in Razor views. | `net8.0`, `net10.0` |

Both packages ship XML documentation, include comprehensive unit tests (60+ for the core library and 40+ for MVC), and treat warnings as build errors to keep the codebase clean.

## Why Viae.Fluid?

- **GitHub Flavored Markdown**: Uses Markdig's advanced extensions out of the box.
- **Fluid-native filters**: Register once, then call `{{ content | markdown }}` in your templates.
- **Safe MVC rendering**: Returns `HtmlString`/`IHtmlContent` to avoid double-encoding in Razor views.
- **Customizable pipeline**: Opt in to any Markdig extension with a single lambda.
- **Well-tested**: Extensive test suites built with MSTest and FluentAssertions for both packages.

## Getting Started

### Install the packages

```bash
dotnet add package Viae.Fluid.Markdown
# For ASP.NET MVC projects
dotnet add package Viae.Fluid.Markdown.Mvc
```

### Use in non-MVC apps

```csharp
using Fluid;
using Viae.Fluid.Markdown;

var (filter, templateOptions) = FluidMarkdownCoreRegistration.CreateCore();
var parser = new FluidParser();
parser.TryParse("{{ content | markdown }}", out var template, out _);

var context = new TemplateContext(templateOptions);
context.SetValue("content", "# Hello **Markdown**");

var html = await template.RenderAsync(context);
// <h1>Hello <strong>Markdown</strong></h1>
```

### Use in ASP.NET MVC

```csharp
using Viae.Fluid.Markdown.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFluid(options =>
{
    options.AddFluidMarkdownFilters(opts =>
    {
        opts.ConfigurePipeline = pipeline => pipeline.UseAdvancedExtensions();
    });
});
```

```liquid
<!-- Views/Post/Show.liquid -->
<article>
  <h1>{{ post.title }}</h1>
  <div class="content">
    {{ post.content | markdown }}
  </div>
</article>
```

## Repository Layout

- `src/Viae.Fluid.Markdown` – Core Markdown filter and helpers.
- `src/Viae.Fluid.Markdown.Mvc` – ASP.NET MVC integration and value converters.
- `tests/Viae.Fluid.Markdown.Tests` – Unit tests for the core filter.
- `tests/Viae.Fluid.Markdown.Mvc.Tests` – Unit tests for the MVC adapter.

## Development

Clone the repository and restore dependencies:

```bash
git clone https://github.com/curt/Viae.Fluid.git
cd Viae.Fluid
dotnet restore
```

### Run the test suites

```bash
dotnet test
```

The solution uses [Nerdbank.GitVersioning](version.json) for deterministic build numbers, so package versions match the current Git height.

## Contributing

Issues and pull requests are welcome. Please run `dotnet test` before submitting changes, follow the existing code style (enforced via `Directory.Build.props`), and review the guidelines in [`CONTRIBUTING.md`](CONTRIBUTING.md).

## License

Distributed under the [MIT License](LICENSE).
