// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

namespace Viae.Fluid.Markdown;

/// <summary>
/// A Fluid template filter that renders Markdown content to HTML.
/// </summary>
/// <param name="renderer">The Markdown renderer to use for converting Markdown to HTML.</param>
/// <remarks>
/// This filter can be registered with Fluid's TemplateOptions to enable Markdown rendering
/// in Liquid templates using the 'markdown' or 'markdownify' filter syntax.
/// The rendered output is returned as a plain string value, suitable for use in non-MVC contexts.
/// For ASP.NET MVC applications, use <see cref="MarkdownMvcAdapter"/> instead to get HtmlString output.
/// </remarks>
/// <example>
/// <code>
/// var renderer = new MarkdigMarkdownRenderer(pipeline);
/// var filter = new MarkdownFilter(renderer);
/// var options = new TemplateOptions();
/// filter.Register(options);
///
/// // In template: {{ content | markdown }}
/// </code>
/// </example>
public sealed class MarkdownFilter(IMarkdownRenderer renderer)
{
    /// <summary>
    /// Invokes the Markdown filter to render the input value as HTML.
    /// </summary>
    /// <param name="input">The input value containing Markdown text. Null values are treated as empty strings.</param>
    /// <param name="args">Additional filter arguments (not used by this filter).</param>
    /// <param name="ctx">The Fluid template context.</param>
    /// <returns>A <see cref="FluidValue"/> containing the rendered HTML as a string.</returns>
    public ValueTask<FluidValue> InvokeAsync(
        FluidValue input,
        FilterArguments args,
        TemplateContext ctx
    )
    {
        var html = renderer.ToHtml(input?.ToStringValue() ?? string.Empty);
        // In pure Fluid tests (no MVC), return as a normal string.
        // Your test can assert the HTML string directly.
        return new ValueTask<FluidValue>(FluidValue.Create(html, ctx.Options));
    }

    /// <summary>
    /// Registers this filter with Fluid's template options.
    /// </summary>
    /// <param name="options">The Fluid template options to register the filter with.</param>
    /// <remarks>
    /// This method registers the filter under two names:
    /// <list type="bullet">
    /// <item><description>'markdown' - The standard filter name</description></item>
    /// <item><description>'markdownify' - An alias compatible with Jekyll/Liquid conventions</description></item>
    /// </list>
    /// </remarks>
    public void Register(TemplateOptions options)
    {
        options.Filters.AddFilter("markdown", InvokeAsync);
        options.Filters.AddFilter("markdownify", InvokeAsync);
    }
}
