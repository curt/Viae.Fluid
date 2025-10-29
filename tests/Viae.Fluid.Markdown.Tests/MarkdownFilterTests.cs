// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using Viae.Fluid.Markdown.Tests.TestHelpers;

namespace Viae.Fluid.Markdown.Tests;

[TestClass]
public sealed class MarkdownFilterTests
{
    [TestMethod]
    public async Task InvokeAsync_WithBoldMarkdown_ReturnsFluidValueWithBoldHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var input = FluidValue.Create("**bold**", options);
        var args = FilterArguments.Empty;

        // Act
        var result = await filter.InvokeAsync(input, args, context);

        // Assert
        result.ToStringValue().Should().Contain("<strong>bold</strong>");
    }

    [TestMethod]
    public async Task InvokeAsync_WithNullInput_ReturnsEmptyHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var args = FilterArguments.Empty;

        // Act
        var result = await filter.InvokeAsync(null!, args, context);

        // Assert
        result.ToStringValue().Should().BeEmpty();
    }

    [TestMethod]
    public async Task InvokeAsync_WithComplexMarkdown_ReturnsComplexHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "# Heading\n\n**Bold** and *italic* text.";
        var input = FluidValue.Create(markdown, options);
        var args = FilterArguments.Empty;

        // Act
        var result = await filter.InvokeAsync(input, args, context);

        // Assert
        result
            .ToStringValue()
            .Should()
            .Contain("<h1")
            .And.Contain("<strong>Bold</strong>")
            .And.Contain("<em>italic</em>");
    }

    [TestMethod]
    public void Register_WithTemplateOptions_AddsMarkdownFilter()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();

        // Act
        filter.Register(options);

        // Assert
        FluidTestHelper.HasFilter(options, "markdown").Should().BeTrue();
    }

    [TestMethod]
    public void Register_WithTemplateOptions_AddsMarkdownifyFilter()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();

        // Act
        filter.Register(options);

        // Assert
        FluidTestHelper.HasFilter(options, "markdownify").Should().BeTrue();
    }

    [TestMethod]
    public async Task Register_MarkdownFilter_RendersCorrectly()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();
        filter.Register(options);
        var context = FluidTestHelper.CreateContext(options);

        // Act
        var output = await FluidTestHelper.RenderTemplateAsync(
            "{{ '**bold**' | markdown }}",
            context
        );

        // Assert
        output.Should().Contain("<strong>bold</strong>");
    }

    [TestMethod]
    public async Task Register_MarkdownifyFilter_RendersCorrectly()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();
        filter.Register(options);
        var context = FluidTestHelper.CreateContext(options);

        // Act
        var output = await FluidTestHelper.RenderTemplateAsync(
            "{{ '*italic*' | markdownify }}",
            context
        );

        // Assert
        output.Should().Contain("<em>italic</em>");
    }

    [TestMethod]
    public async Task InvokeAsync_WithLinkMarkdown_ReturnsSanitizedHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "[Link](https://example.com)";
        var input = FluidValue.Create(markdown, options);
        var args = FilterArguments.Empty;

        // Act
        var result = await filter.InvokeAsync(input, args, context);

        // Assert
        result.ToStringValue().Should().Contain("<a").And.Contain("href=\"https://example.com\"");
    }

    [TestMethod]
    public async Task InvokeAsync_WithCodeBlock_ReturnsCodeHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "`code block`";
        var input = FluidValue.Create(markdown, options);
        var args = FilterArguments.Empty;

        // Act
        var result = await filter.InvokeAsync(input, args, context);

        // Assert
        result.ToStringValue().Should().Contain("<code>code block</code>");
    }

    [TestMethod]
    public async Task InvokeAsync_WithListMarkdown_ReturnsListHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var filter = new MarkdownFilter(renderer);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "- Item 1\n- Item 2";
        var input = FluidValue.Create(markdown, options);
        var args = FilterArguments.Empty;

        // Act
        var result = await filter.InvokeAsync(input, args, context);

        // Assert
        result
            .ToStringValue()
            .Should()
            .Contain("<ul>")
            .And.Contain("<li>Item 1</li>")
            .And.Contain("<li>Item 2</li>");
    }
}
