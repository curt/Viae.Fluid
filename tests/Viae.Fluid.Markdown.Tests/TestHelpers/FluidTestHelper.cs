// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

namespace Viae.Fluid.Markdown.Tests.TestHelpers;

/// <summary>
/// Base helper class for Fluid template testing to ensure DRY compliance.
/// </summary>
public static class FluidTestHelper
{
    /// <summary>
    /// Parses a Fluid template string and returns the compiled template.
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
    /// Creates a TemplateContext with the provided TemplateOptions.
    /// </summary>
    /// <param name="options">The template options to use.</param>
    /// <returns>A new TemplateContext instance.</returns>
    public static TemplateContext CreateContext(TemplateOptions options)
    {
        return new TemplateContext(options);
    }

    /// <summary>
    /// Creates a default Markdown pipeline with advanced extensions.
    /// </summary>
    /// <returns>A configured MarkdownPipeline.</returns>
    public static MarkdownPipeline CreateDefaultPipeline()
    {
        return new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }

    /// <summary>
    /// Creates a custom Markdown pipeline with the specified configuration.
    /// </summary>
    /// <param name="configure">The configuration function.</param>
    /// <returns>A configured MarkdownPipeline.</returns>
    public static MarkdownPipeline CreateCustomPipeline(
        Func<MarkdownPipelineBuilder, MarkdownPipelineBuilder> configure
    )
    {
        var builder = new MarkdownPipelineBuilder();
        return configure(builder).Build();
    }

    /// <summary>
    /// Checks if a filter is registered in the template options.
    /// </summary>
    /// <param name="options">The template options to check.</param>
    /// <param name="filterName">The name of the filter to check for.</param>
    /// <returns>True if the filter is registered, false otherwise.</returns>
    public static bool HasFilter(TemplateOptions options, string filterName)
    {
        // Try to get the filter - if it returns null, the filter doesn't exist
        return options.Filters.TryGetValue(filterName, out _);
    }
}
