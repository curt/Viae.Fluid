// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

namespace Viae.Fluid.Markdown;

/// <summary>
/// Configuration options for the Markdown filter.
/// </summary>
/// <remarks>
/// This class allows customization of the Markdig pipeline used for rendering Markdown.
/// By default, it configures the pipeline with advanced extensions enabled, which includes
/// support for tables, task lists, strikethrough, and other GitHub Flavored Markdown features.
/// </remarks>
/// <example>
/// <code>
/// // Use default configuration with advanced extensions
/// var options = new MarkdownFilterOptions();
///
/// // Customize the pipeline
/// var customOptions = new MarkdownFilterOptions
/// {
///     ConfigurePipeline = builder => builder
///         .UseAdvancedExtensions()
///         .UseEmphasisExtras()
/// };
/// </code>
/// </example>
public sealed class MarkdownFilterOptions
{
    /// <summary>
    /// Gets or sets a function that configures the Markdig pipeline.
    /// </summary>
    /// <value>
    /// A function that takes a <see cref="MarkdownPipelineBuilder"/> and returns a configured builder.
    /// The default configuration uses <see cref="MarkdownExtensions.UseAdvancedExtensions"/>,
    /// which enables GitHub Flavored Markdown features including:
    /// <list type="bullet">
    /// <item><description>Tables (pipe tables and grid tables)</description></item>
    /// <item><description>Task lists (- [ ] and - [x])</description></item>
    /// <item><description>Strikethrough (~~text~~)</description></item>
    /// <item><description>Auto-identifiers for headings</description></item>
    /// <item><description>Abbreviations</description></item>
    /// <item><description>Definition lists</description></item>
    /// <item><description>Footnotes</description></item>
    /// <item><description>And more</description></item>
    /// </list>
    /// </value>
    /// <remarks>
    /// You can customize this to enable or disable specific Markdown extensions
    /// by providing your own configuration function.
    /// </remarks>
    public Func<MarkdownPipelineBuilder, MarkdownPipelineBuilder> ConfigurePipeline { get; set; } =
        b => b.UseAdvancedExtensions();
}
