// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

namespace Viae.Fluid.Markdown;

/// <summary>
/// A Markdown renderer implementation using the Markdig library.
/// </summary>
/// <param name="pipeline">The Markdig pipeline configuration to use for rendering.</param>
/// <remarks>
/// This renderer uses Markdig's MarkdownPipeline to process Markdown text.
/// The pipeline determines which Markdown extensions and features are enabled.
/// </remarks>
public sealed class MarkdigMarkdownRenderer(MarkdownPipeline pipeline) : IMarkdownRenderer
{
    /// <inheritdoc/>
    public string ToHtml(string markdown) =>
        Markdig.Markdown.ToHtml(markdown ?? string.Empty, pipeline);
}
