// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using Microsoft.AspNetCore.Html;
using Viae.Fluid.Markdown.Tests.TestHelpers;

namespace Viae.Fluid.Markdown.Tests;

[TestClass]
public sealed class MarkdownMvcAdapterTests
{
    [TestMethod]
    public async Task InvokeAsync_WithBoldMarkdown_ReturnsHtmlStringValue()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var input = FluidValue.Create("**bold**", options);
        var args = FilterArguments.Empty;

        // Act
        var result = await adapter.InvokeAsync(input, args, context);

        // Assert
        result.Should().NotBeNull();
        var objectValue = result.ToObjectValue();
        objectValue.Should().BeOfType<HtmlString>();
        var htmlString = (HtmlString)objectValue;
        htmlString.ToString().Should().Contain("<strong>bold</strong>");
    }

    [TestMethod]
    public async Task InvokeAsync_WithComplexMarkdown_ReturnsHtmlStringWithComplexHtml()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "# Heading\n\n**Bold** and *italic*";
        var input = FluidValue.Create(markdown, options);
        var args = FilterArguments.Empty;

        // Act
        var result = await adapter.InvokeAsync(input, args, context);

        // Assert
        var htmlString = (HtmlString)result.ToObjectValue();
        htmlString
            .ToString()
            .Should()
            .Contain("<h1")
            .And.Contain("<strong>Bold</strong>")
            .And.Contain("<em>italic</em>");
    }

    [TestMethod]
    public async Task InvokeAsync_WithNullInput_ReturnsEmptyHtmlString()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var args = FilterArguments.Empty;

        // Act
        var result = await adapter.InvokeAsync(null!, args, context);

        // Assert
        var htmlString = (HtmlString)result.ToObjectValue();
        htmlString.ToString().Should().BeEmpty();
    }

    [TestMethod]
    public void Register_WithTemplateOptions_AddsMarkdownFilter()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();

        // Act
        adapter.Register(options);

        // Assert
        FluidTestHelper.HasFilter(options, "markdown").Should().BeTrue();
    }

    [TestMethod]
    public void Register_WithTemplateOptions_AddsMarkdownifyFilter()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();

        // Act
        adapter.Register(options);

        // Assert
        FluidTestHelper.HasFilter(options, "markdownify").Should().BeTrue();
    }

    [TestMethod]
    public async Task Register_MarkdownFilter_ReturnsHtmlString()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        adapter.Register(options);

        // Use Fluid to test the filter in a template context
        var context = FluidTestHelper.CreateContext(options);
        var template = FluidTestHelper.ParseTemplate("{{ '**bold**' | markdown }}");

        // Act
        var output = await template.RenderAsync(context);

        // Assert - The output should contain the rendered HTML
        // Note: HtmlString will be encoded in the template output
        output.Should().Contain("<strong>bold</strong>");
    }

    [TestMethod]
    public async Task Register_MarkdownifyFilter_ReturnsHtmlString()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        adapter.Register(options);

        var context = FluidTestHelper.CreateContext(options);
        var template = FluidTestHelper.ParseTemplate("{{ '*italic*' | markdownify }}");

        // Act
        var output = await template.RenderAsync(context);

        // Assert
        output.Should().Contain("<em>italic</em>");
    }

    [TestMethod]
    public async Task InvokeAsync_WithLinkMarkdown_ReturnsHtmlStringWithLink()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "[Link](https://example.com)";
        var input = FluidValue.Create(markdown, options);
        var args = FilterArguments.Empty;

        // Act
        var result = await adapter.InvokeAsync(input, args, context);

        // Assert
        var htmlString = (HtmlString)result.ToObjectValue();
        htmlString.ToString().Should().Contain("<a").And.Contain("href=\"https://example.com\"");
    }

    [TestMethod]
    public async Task InvokeAsync_WithCodeBlock_ReturnsHtmlStringWithCode()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "`code`";
        var input = FluidValue.Create(markdown, options);
        var args = FilterArguments.Empty;

        // Act
        var result = await adapter.InvokeAsync(input, args, context);

        // Assert
        var htmlString = (HtmlString)result.ToObjectValue();
        htmlString.ToString().Should().Contain("<code>code</code>");
    }

    [TestMethod]
    public async Task InvokeAsync_DelegatesToCoreFilter()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var input = FluidValue.Create("**test**", options);
        var args = FilterArguments.Empty;

        // Act - Call both adapter and core filter
        var adapterResult = await adapter.InvokeAsync(input, args, context);
        var coreResult = await coreFilter.InvokeAsync(input, args, context);

        // Assert - Adapter should wrap core filter's output in HtmlString
        var adapterHtml = ((HtmlString)adapterResult.ToObjectValue()).ToString();
        var coreHtml = coreResult.ToStringValue();
        adapterHtml.Should().Be(coreHtml);
    }

    [TestMethod]
    public async Task InvokeAsync_WithMultilineMarkdown_ReturnsProperlyFormattedHtmlString()
    {
        // Arrange
        var pipeline = FluidTestHelper.CreateDefaultPipeline();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var coreFilter = new MarkdownFilter(renderer);
        var adapter = new MarkdownMvcAdapter(coreFilter);
        var options = new TemplateOptions();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "# Title\n\nParagraph 1\n\nParagraph 2";
        var input = FluidValue.Create(markdown, options);
        var args = FilterArguments.Empty;

        // Act
        var result = await adapter.InvokeAsync(input, args, context);

        // Assert
        var htmlString = (HtmlString)result.ToObjectValue();
        htmlString
            .ToString()
            .Should()
            .Contain("<h1")
            .And.Contain("<p>Paragraph 1</p>")
            .And.Contain("<p>Paragraph 2</p>");
    }
}
