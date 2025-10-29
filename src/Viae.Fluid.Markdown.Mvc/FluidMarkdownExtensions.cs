// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using Microsoft.AspNetCore.Html;

namespace Viae.Fluid.Markdown.Mvc;

/// <summary>
/// Extension methods for integrating Markdown rendering into ASP.NET MVC Fluid view engine.
/// </summary>
/// <remarks>
/// This class provides fluent configuration methods for adding Markdown filter support
/// to the Fluid MVC view engine with proper HTML encoding handling for ASP.NET MVC/Razor.
/// </remarks>
public static class FluidMarkdownExtensions
{
    /// <summary>
    /// Adds Markdown filters to the Fluid view engine options for ASP.NET MVC applications.
    /// </summary>
    /// <param name="o">The <see cref="FluidViewEngineOptions"/> to configure.</param>
    /// <param name="configure">
    /// Optional configuration action for customizing the Markdown pipeline.
    /// If not provided, default options with advanced extensions will be used.
    /// </param>
    /// <returns>The configured <see cref="FluidViewEngineOptions"/> for fluent chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method performs the following configuration steps:
    /// <list type="number">
    /// <item><description>Creates and configures <see cref="MarkdownFilterOptions"/> using the provided action</description></item>
    /// <item><description>Adds a value converter to handle <see cref="IHtmlContent"/> as raw HTML in Fluid templates</description></item>
    /// <item><description>Builds a Markdig pipeline with the configured options</description></item>
    /// <item><description>Creates a <see cref="MarkdigMarkdownRenderer"/> with the pipeline</description></item>
    /// <item><description>Creates a <see cref="MarkdownFilter"/> and wraps it in <see cref="MarkdownMvcAdapter"/></description></item>
    /// <item><description>Registers the MVC adapter with template options under 'markdown' and 'markdownify' filter names</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The MVC adapter ensures that rendered Markdown HTML is wrapped in <see cref="HtmlString"/>
    /// to prevent double-encoding when rendered in Razor views.
    /// </para>
    /// <para>
    /// The value converter registration ensures that any <see cref="IHtmlContent"/> values
    /// (including <see cref="HtmlString"/>) are properly handled as pre-encoded HTML by Fluid.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // In Program.cs or Startup.cs
    /// services.AddFluid(options =>
    /// {
    ///     // Add Markdown filters with default configuration
    ///     options.AddFluidMarkdownFilters();
    ///
    ///     // Or with custom configuration
    ///     options.AddFluidMarkdownFilters(opts =>
    ///     {
    ///         opts.ConfigurePipeline = builder => builder
    ///             .UseAdvancedExtensions()
    ///             .UseEmphasisExtras();
    ///     });
    /// });
    /// </code>
    /// </example>
    public static FluidViewEngineOptions AddFluidMarkdownFilters(
        this FluidViewEngineOptions o,
        Action<MarkdownFilterOptions>? configure = null
    )
    {
        // Build per-app singletons
        var opts = new MarkdownFilterOptions();
        configure?.Invoke(opts);

        // Ensure IHtmlContent is handled as raw HTML by Fluid
        o.TemplateOptions.ValueConverters.Add(x =>
            x is IHtmlContent h ? new HtmlContentFluidValue(h) : null
        );

        var pipeline = opts.ConfigurePipeline(new MarkdownPipelineBuilder()).Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var core = new MarkdownFilter(renderer);

        // If you're in MVC, prefer the adapter to return IHtmlContent
        var adapter = new MarkdownMvcAdapter(core);
        adapter.Register(o.TemplateOptions);

        return o;
    }
}
