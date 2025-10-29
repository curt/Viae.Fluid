// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using System.Globalization;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace Viae.Fluid.Markdown.Mvc;

/// <summary>
/// A Fluid value wrapper for <see cref="IHtmlContent"/> that preserves HTML encoding semantics.
/// </summary>
/// <param name="value">The HTML content to wrap. Null values are treated as empty HTML strings.</param>
/// <remarks>
/// <para>
/// This class enables Fluid templates to work seamlessly with ASP.NET MVC's <see cref="IHtmlContent"/>
/// types (such as <see cref="HtmlString"/>) by providing a bridge between Fluid's value system
/// and ASP.NET's HTML content system.
/// </para>
/// <para>
/// When an <see cref="IHtmlContent"/> value is converted to a <see cref="HtmlContentFluidValue"/>,
/// it retains its "already-encoded" semantics. This prevents double-encoding when the value
/// is written to output in Razor views or other ASP.NET MVC contexts.
/// </para>
/// <para>
/// This class is automatically used when you configure the Fluid view engine with
/// <see cref="FluidMarkdownExtensions.AddFluidMarkdownFilters"/>, which registers a value
/// converter for <see cref="IHtmlContent"/> types.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Value converter registration (done automatically by AddFluidMarkdownFilters)
/// options.TemplateOptions.ValueConverters.Add(x =>
///     x is IHtmlContent h ? new HtmlContentFluidValue(h) : null
/// );
///
/// // The converter enables this workflow:
/// // 1. Markdown filter returns HtmlString
/// // 2. HtmlString is wrapped in HtmlContentFluidValue
/// // 3. Value is written to output without double-encoding
/// </code>
/// </example>
public sealed class HtmlContentFluidValue(IHtmlContent value) : FluidValue
{
    private readonly IHtmlContent _value = value ?? new HtmlString(string.Empty);

    /// <summary>
    /// Gets the Fluid value type, which is always <see cref="FluidValues.String"/> for HTML content.
    /// </summary>
    public override FluidValues Type => FluidValues.String;

    /// <summary>
    /// Converts the HTML content to a boolean value.
    /// </summary>
    /// <returns>Always returns <c>true</c> for non-null HTML content.</returns>
    /// <remarks>
    /// HTML content is considered truthy in Fluid template conditions,
    /// even if it represents an empty string.
    /// </remarks>
    public override bool ToBooleanValue() => true;

    /// <summary>
    /// Converts the HTML content to a numeric value.
    /// </summary>
    /// <returns>Always returns <c>0</c> as HTML content is not numeric.</returns>
    /// <remarks>
    /// HTML content cannot be meaningfully converted to a number,
    /// so this method returns the default numeric value of zero.
    /// </remarks>
    public override decimal ToNumberValue() => 0m;

    /// <summary>
    /// Converts the HTML content to its string representation.
    /// </summary>
    /// <returns>The HTML content as a string with proper HTML encoding applied.</returns>
    /// <remarks>
    /// This method writes the <see cref="IHtmlContent"/> to a string writer using
    /// <see cref="HtmlEncoder.Default"/>, preserving the HTML encoding semantics.
    /// The resulting string contains the actual HTML markup.
    /// </remarks>
    public override string ToStringValue()
    {
        using var sw = new StringWriter(CultureInfo.InvariantCulture);
        _value.WriteTo(sw, HtmlEncoder.Default);
        return sw.ToString();
    }

    /// <summary>
    /// Returns the underlying <see cref="IHtmlContent"/> object.
    /// </summary>
    /// <returns>The wrapped <see cref="IHtmlContent"/> instance.</returns>
    public override object ToObjectValue() => _value;

    /// <summary>
    /// Enumerates the value for iteration in Fluid templates.
    /// </summary>
    /// <param name="context">The Fluid template context.</param>
    /// <returns>An empty enumerable as HTML content is not iterable.</returns>
    /// <remarks>
    /// HTML content cannot be meaningfully enumerated, so this method returns an empty collection.
    /// Attempting to iterate over HTML content in a template will result in no iterations.
    /// </remarks>
    public override IEnumerable<FluidValue> Enumerate(TemplateContext context) => [];

    /// <summary>
    /// Determines whether this HTML content value is equal to another Fluid value.
    /// </summary>
    /// <param name="other">The other Fluid value to compare with.</param>
    /// <returns>
    /// <c>true</c> if the values are reference-equal or have identical string representations;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Equality is determined by comparing the string representations of the HTML content
    /// using ordinal (case-sensitive, culture-invariant) comparison.
    /// </remarks>
    public override bool Equals(FluidValue other) =>
        ReferenceEquals(this, other)
        || string.Equals(ToStringValue(), other?.ToStringValue(), StringComparison.Ordinal);

    /// <summary>
    /// Writes the HTML content to a text writer asynchronously.
    /// </summary>
    /// <param name="writer">The text writer to write the HTML content to.</param>
    /// <param name="encoder">
    /// The text encoder to use. If an <see cref="HtmlEncoder"/> is provided, it will be used;
    /// otherwise, <see cref="HtmlEncoder.Default"/> is used.
    /// </param>
    /// <param name="cultureInfo">The culture info (not used for HTML content).</param>
    /// <returns>A completed <see cref="ValueTask"/> as the operation is synchronous.</returns>
    /// <remarks>
    /// <para>
    /// This method is called by Fluid when rendering the value to output.
    /// It delegates directly to the <see cref="IHtmlContent.WriteTo"/> method,
    /// which preserves the HTML encoding semantics and prevents double-encoding.
    /// </para>
    /// <para>
    /// Although this method is asynchronous, the actual write operation is synchronous
    /// as <see cref="IHtmlContent.WriteTo"/> is a synchronous method.
    /// </para>
    /// </remarks>
    public override ValueTask WriteToAsync(
        TextWriter writer,
        TextEncoder encoder,
        CultureInfo cultureInfo
    )
    {
        _value.WriteTo(writer, encoder as HtmlEncoder ?? HtmlEncoder.Default);
        return ValueTask.CompletedTask;
    }
}
