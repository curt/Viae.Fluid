// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using Viae.Fluid.Markdown.Tests.TestHelpers;

namespace Viae.Fluid.Markdown.Tests;

[TestClass]
public sealed class MarkdigMarkdownRendererTests
{
    [TestMethod]
    public void ToHtml_WithBoldMarkdown_ReturnsBoldHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Act
        var result = renderer.ToHtml("**bold text**");

        // Assert
        result.Should().Contain("<strong>bold text</strong>");
    }

    [TestMethod]
    public void ToHtml_WithItalicMarkdown_ReturnsItalicHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Act
        var result = renderer.ToHtml("*italic text*");

        // Assert
        result.Should().Contain("<em>italic text</em>");
    }

    [TestMethod]
    public void ToHtml_WithHeadingMarkdown_ReturnsHeadingHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Act
        var result = renderer.ToHtml("# Heading 1");

        // Assert
        result.Should().Contain("<h1").And.Contain("Heading 1");
    }

    [TestMethod]
    public void ToHtml_WithLinkMarkdown_ReturnsLinkHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Act
        var result = renderer.ToHtml("[Link Text](https://example.com)");

        // Assert
        result
            .Should()
            .Contain("<a")
            .And.Contain("href=\"https://example.com\"")
            .And.Contain("Link Text");
    }

    [TestMethod]
    public void ToHtml_WithCodeBlockMarkdown_ReturnsCodeHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Act
        var result = renderer.ToHtml("`inline code`");

        // Assert
        result.Should().Contain("<code>inline code</code>");
    }

    [TestMethod]
    public void ToHtml_WithUnorderedListMarkdown_ReturnsListHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var markdown = "- Item 1\n- Item 2";

        // Act
        var result = renderer.ToHtml(markdown);

        // Assert
        result
            .Should()
            .Contain("<ul>")
            .And.Contain("<li>Item 1</li>")
            .And.Contain("<li>Item 2</li>");
    }

    [TestMethod]
    public void ToHtml_WithOrderedListMarkdown_ReturnsOrderedListHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var markdown = "1. First\n2. Second";

        // Act
        var result = renderer.ToHtml(markdown);

        // Assert
        result
            .Should()
            .Contain("<ol>")
            .And.Contain("<li>First</li>")
            .And.Contain("<li>Second</li>");
    }

    [TestMethod]
    public void ToHtml_WithEmptyString_ReturnsEmptyString()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Act
        var result = renderer.ToHtml(string.Empty);

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void ToHtml_WithNull_ReturnsEmptyString()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Act
        var result = renderer.ToHtml(null!);

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void ToHtml_WithTableMarkdown_ReturnsTableHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var markdown = "| Header 1 | Header 2 |\n|----------|----------|\n| Cell 1   | Cell 2   |";

        // Act
        var result = renderer.ToHtml(markdown);

        // Assert
        result
            .Should()
            .Contain("<table>")
            .And.Contain("<th>Header 1</th>")
            .And.Contain("<td>Cell 1</td>");
    }

    [TestMethod]
    public void ToHtml_WithStrikethroughMarkdown_ReturnsStrikethroughHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Act
        var result = renderer.ToHtml("~~strikethrough~~");

        // Assert
        result.Should().Contain("<del>strikethrough</del>");
    }

    [TestMethod]
    public void ToHtml_WithTaskListMarkdown_ReturnsTaskListHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var markdown = "- [ ] Unchecked\n- [x] Checked";

        // Act
        var result = renderer.ToHtml(markdown);

        // Assert
        result.Should().Contain("<input").And.Contain("type=\"checkbox\"");
    }

    [TestMethod]
    public void ToHtml_WithCustomPipeline_UsesCustomConfiguration()
    {
        // Arrange - Create pipeline without advanced extensions
        var pipeline = new MarkdownPipelineBuilder().Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Act
        var result = renderer.ToHtml("~~strikethrough~~");

        // Assert - Without advanced extensions, strikethrough won't be processed
        result.Should().NotContain("<del>").And.Contain("~~strikethrough~~");
    }

    [TestMethod]
    public void ToHtml_WithMultipleMarkdownFeatures_ReturnsComplexHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var markdown = "# Title\n\n**Bold** and *italic* with [link](https://example.com)";

        // Act
        var result = renderer.ToHtml(markdown);

        // Assert
        result
            .Should()
            .Contain("<h1")
            .And.Contain("<strong>Bold</strong>")
            .And.Contain("<em>italic</em>")
            .And.Contain("<a")
            .And.Contain("href=\"https://example.com\"");
    }
}
