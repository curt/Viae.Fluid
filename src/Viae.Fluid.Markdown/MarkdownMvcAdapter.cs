// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using Microsoft.AspNetCore.Html;

namespace Viae.Fluid.Markdown;

/// <summary>
/// An ASP.NET MVC-specific adapter for the Markdown filter that returns HtmlString values.
/// </summary>
/// <param name="core">The core <see cref="MarkdownFilter"/> to delegate rendering to.</param>
/// <remarks>
/// <para>
/// This adapter wraps <see cref="MarkdownFilter"/> and converts its output to <see cref="HtmlString"/>,
/// which prevents double-encoding of HTML in ASP.NET MVC Razor views.
/// </para>
/// <para>
/// When rendering Markdown in MVC views, the raw HTML output needs to be wrapped in an HtmlString
/// to signal to Razor that the content is already HTML-encoded and should not be encoded again.
/// Without this wrapper, characters like &lt; and &gt; would be displayed as text rather than rendered as HTML tags.
/// </para>
/// <para>
/// <strong>Usage Guidance:</strong>
/// <list type="bullet">
/// <item><description>Use <see cref="MarkdownMvcAdapter"/> in ASP.NET MVC applications with Razor views</description></item>
/// <item><description>Use <see cref="MarkdownFilter"/> directly in non-MVC contexts or when you want plain string output</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // In ASP.NET MVC Startup or Program.cs
/// var (coreFilter, _) = FluidMarkdownCoreRegistration.CreateCore();
/// var mvcAdapter = new MarkdownMvcAdapter(coreFilter);
/// var options = new TemplateOptions();
/// mvcAdapter.Register(options);
///
/// // In Razor view using Fluid:
/// // @await RenderAsync("{{ content | markdown }}")
/// // The output will be properly rendered as HTML, not escaped
/// </code>
/// </example>
public sealed class MarkdownMvcAdapter(MarkdownFilter core)
{
    /// <summary>
    /// Invokes the Markdown filter and wraps the result in an <see cref="HtmlString"/>.
    /// </summary>
    /// <param name="input">The input value containing Markdown text. Null values are treated as empty strings.</param>
    /// <param name="args">Additional filter arguments (not used by this filter).</param>
    /// <param name="ctx">The Fluid template context.</param>
    /// <returns>
    /// A <see cref="FluidValue"/> containing an <see cref="HtmlString"/> with the rendered HTML.
    /// This prevents the HTML from being double-encoded when rendered in MVC views.
    /// </returns>
    /// <remarks>
    /// This method delegates to the core <see cref="MarkdownFilter"/> for the actual Markdown rendering,
    /// then wraps the resulting HTML string in an <see cref="HtmlString"/> to mark it as pre-encoded HTML.
    /// </remarks>
    public ValueTask<FluidValue> InvokeAsync(
        FluidValue input,
        FilterArguments args,
        TemplateContext ctx
    )
    {
        var html = ((StringValue)core.InvokeAsync(input, args, ctx).Result).ToStringValue();
        return new ValueTask<FluidValue>(FluidValue.Create(new HtmlString(html), ctx.Options));
    }

    /// <summary>
    /// Registers this adapter with Fluid's template options for use in MVC applications.
    /// </summary>
    /// <param name="options">The Fluid template options to register the adapter with.</param>
    /// <remarks>
    /// This method registers the adapter under two names:
    /// <list type="bullet">
    /// <item><description>'markdown' - The standard filter name</description></item>
    /// <item><description>'markdownify' - An alias compatible with Jekyll/Liquid conventions</description></item>
    /// </list>
    /// Both filter names will return <see cref="HtmlString"/> values suitable for MVC Razor views.
    /// </remarks>
    public void Register(TemplateOptions options)
    {
        options.Filters.AddFilter("markdown", InvokeAsync);
        options.Filters.AddFilter("markdownify", InvokeAsync);
    }
}
