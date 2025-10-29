// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using System.Text.Encodings.Web;

namespace Viae.Fluid.Markdown.Mvc.Tests.TestHelpers;

/// <summary>
/// Helper class for MVC Fluid Markdown testing to ensure DRY compliance.
/// </summary>
public static class MvcTestHelper
{
    /// <summary>
    /// Creates a FluidViewEngineOptions instance for testing.
    /// </summary>
    /// <returns>A new FluidViewEngineOptions instance.</returns>
    public static FluidViewEngineOptions CreateFluidViewEngineOptions()
    {
        return new FluidViewEngineOptions();
    }

    /// <summary>
    /// Converts IHtmlContent to its string representation.
    /// </summary>
    /// <param name="htmlContent">The HTML content to convert.</param>
    /// <returns>The string representation of the HTML content.</returns>
    public static string HtmlContentToString(IHtmlContent htmlContent)
    {
        using var writer = new StringWriter();
        htmlContent.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }

    /// <summary>
    /// Creates a TemplateContext with the provided options.
    /// </summary>
    /// <param name="options">The template options to use.</param>
    /// <returns>A new TemplateContext instance.</returns>
    public static TemplateContext CreateContext(TemplateOptions options)
    {
        return new TemplateContext(options);
    }

    /// <summary>
    /// Parses a Fluid template string.
    /// </summary>
    /// <param name="templateString">The template string to parse.</param>
    /// <returns>The parsed template.</returns>
    /// <exception cref="InvalidOperationException">Thrown when template parsing fails.</exception>
    public static IFluidTemplate ParseTemplate(string templateString)
    {
        var parser = new FluidParser();
        return !parser.TryParse(templateString, out var template, out var error)
            ? throw new InvalidOperationException($"Template parsing failed: {error}")
            : template;
    }

    /// <summary>
    /// Parses and renders a Fluid template with the given context.
    /// </summary>
    /// <param name="templateString">The template string to parse and render.</param>
    /// <param name="context">The template context to use for rendering.</param>
    /// <returns>The rendered output as a string.</returns>
    public static async Task<string> RenderTemplateAsync(
        string templateString,
        TemplateContext context
    )
    {
        var template = ParseTemplate(templateString);
        return await template.RenderAsync(context);
    }

    /// <summary>
    /// Checks if a filter is registered in the template options.
    /// </summary>
    /// <param name="options">The template options to check.</param>
    /// <param name="filterName">The name of the filter to check for.</param>
    /// <returns>True if the filter is registered, false otherwise.</returns>
    public static bool HasFilter(TemplateOptions options, string filterName)
    {
        return options.Filters.TryGetValue(filterName, out _);
    }

    /// <summary>
    /// Creates a default MarkdownPipeline with advanced extensions.
    /// </summary>
    /// <returns>A configured MarkdownPipeline.</returns>
    public static MarkdownPipeline CreateDefaultPipeline()
    {
        return new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }
}
