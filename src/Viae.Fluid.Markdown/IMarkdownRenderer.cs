// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

namespace Viae.Fluid.Markdown;

/// <summary>
/// Defines a contract for rendering Markdown content to HTML.
/// </summary>
public interface IMarkdownRenderer
{
    /// <summary>
    /// Converts Markdown text to HTML.
    /// </summary>
    /// <param name="markdown">The Markdown text to convert. Null values should be treated as empty strings.</param>
    /// <returns>The rendered HTML string.</returns>
    string ToHtml(string markdown);
}
