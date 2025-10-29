// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

namespace Viae.Fluid.Markdown;

/// <summary>
/// Provides factory methods for creating and configuring Markdown filters for Fluid templates.
/// </summary>
/// <remarks>
/// This class simplifies the setup of Markdown rendering in Fluid templates by providing
/// a single method that creates a configured filter and template options together.
/// This is the recommended way to set up Markdown support in non-MVC Fluid applications.
/// </remarks>
/// <example>
/// <code>
/// // Basic usage with default options
/// var (filter, options) = FluidMarkdownCoreRegistration.CreateCore();
/// var context = new TemplateContext(options);
/// var template = FluidTemplate.Parse("{{ content | markdown }}");
/// var output = await template.RenderAsync(context);
///
/// // Custom configuration
/// var customOptions = new MarkdownFilterOptions
/// {
///     ConfigurePipeline = builder => builder.UseEmphasisExtras()
/// };
/// var (customFilter, templateOptions) = FluidMarkdownCoreRegistration.CreateCore(customOptions);
/// </code>
/// </example>
public static class FluidMarkdownCoreRegistration
{
    /// <summary>
    /// Creates a configured Markdown filter and template options.
    /// </summary>
    /// <param name="opts">
    /// Optional configuration for the Markdown filter. If null, default options are used
    /// which enable Markdig's advanced extensions (GitHub Flavored Markdown features).
    /// </param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item><description>A <see cref="MarkdownFilter"/> configured with the specified options</description></item>
    /// <item><description>A <see cref="TemplateOptions"/> instance with the filter registered under 'markdown' and 'markdownify' names</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// This method performs the following steps:
    /// <list type="number">
    /// <item><description>Creates or uses the provided <see cref="MarkdownFilterOptions"/></description></item>
    /// <item><description>Builds a Markdig pipeline using the options</description></item>
    /// <item><description>Creates a <see cref="MarkdigMarkdownRenderer"/> with the pipeline</description></item>
    /// <item><description>Creates a <see cref="MarkdownFilter"/> with the renderer</description></item>
    /// <item><description>Registers the filter with new <see cref="TemplateOptions"/></description></item>
    /// </list>
    /// The returned TemplateOptions should be used when creating Fluid template contexts.
    /// </remarks>
    public static (MarkdownFilter filter, TemplateOptions options) CreateCore(
        MarkdownFilterOptions? opts = null
    )
    {
        opts ??= new MarkdownFilterOptions();
        var pipeline = opts.ConfigurePipeline(new MarkdownPipelineBuilder()).Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var topts = new TemplateOptions();
        filter.Register(topts);
        return (filter, topts);
    }
}
