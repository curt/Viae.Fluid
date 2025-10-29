// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using Viae.Fluid.Markdown.Mvc.Tests.TestHelpers;

namespace Viae.Fluid.Markdown.Mvc.Tests;

[TestClass]
public sealed class FluidMarkdownExtensionsTests
{
    [TestMethod]
    public void AddFluidMarkdownFilters_WithDefaultOptions_ReturnsOptions()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();

        // Act
        var result = options.AddFluidMarkdownFilters();

        // Assert
        result
            .Should()
            .BeSameAs(options, "method should return the same options for fluent chaining");
    }

    [TestMethod]
    public void AddFluidMarkdownFilters_RegistersMarkdownFilter()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();

        // Act
        options.AddFluidMarkdownFilters();

        // Assert
        MvcTestHelper
            .HasFilter(options.TemplateOptions, "markdown")
            .Should()
            .BeTrue("markdown filter should be registered");
    }

    [TestMethod]
    public void AddFluidMarkdownFilters_RegistersMarkdownifyFilter()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();

        // Act
        options.AddFluidMarkdownFilters();

        // Assert
        MvcTestHelper
            .HasFilter(options.TemplateOptions, "markdownify")
            .Should()
            .BeTrue("markdownify filter should be registered");
    }

    [TestMethod]
    public void AddFluidMarkdownFilters_RegistersIHtmlContentValueConverter()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();

        // Act
        options.AddFluidMarkdownFilters();

        // Assert
        options
            .TemplateOptions.ValueConverters.Should()
            .NotBeEmpty("value converters should be registered");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_MarkdownFilter_RendersHtml()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            "{{ '**bold**' | markdown }}",
            context
        );

        // Assert
        output.Should().Contain("<strong>bold</strong>");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_MarkdownifyFilter_RendersHtml()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            "{{ '*italic*' | markdownify }}",
            context
        );

        // Assert
        output.Should().Contain("<em>italic</em>");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_RendersComplexMarkdown()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);
        var markdown = "# Heading\n\n**Bold** and *italic*";

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            $"{{{{ '{markdown}' | markdown }}}}",
            context
        );

        // Assert
        output
            .Should()
            .Contain("<h1")
            .And.Contain("<strong>Bold</strong>")
            .And.Contain("<em>italic</em>");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_WithDefaultOptions_SupportsAdvancedExtensions()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);

        // Act - Test strikethrough (part of advanced extensions)
        var output = await MvcTestHelper.RenderTemplateAsync(
            "{{ '~~strikethrough~~' | markdown }}",
            context
        );

        // Assert
        output.Should().Contain("<del>strikethrough</del>");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_WithDefaultOptions_SupportsTables()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);
        var markdown = "| Header |\n|--------|\n| Cell   |";

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            $"{{{{ '{markdown}' | markdown }}}}",
            context
        );

        // Assert
        output.Should().Contain("<table>");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_WithDefaultOptions_SupportsTaskLists()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);
        var markdown = "- [x] Checked";

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            $"{{{{ '{markdown}' | markdown }}}}",
            context
        );

        // Assert
        output.Should().Contain("<input").And.Contain("type=\"checkbox\"");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_WithCustomConfiguration_UsesCustomPipeline()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters(opts =>
        {
            opts.ConfigurePipeline = b => b; // No extensions
        });
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            "{{ '~~strikethrough~~' | markdown }}",
            context
        );

        // Assert
        output
            .Should()
            .NotContain(
                "<del>",
                "custom pipeline without extensions should not support strikethrough"
            );
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_WithCustomConfiguration_CanEnableSpecificExtensions()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters(opts =>
        {
            opts.ConfigurePipeline = b => b.UseEmphasisExtras();
        });
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            "{{ '==marked text==' | markdown }}",
            context
        );

        // Assert
        output.Should().Contain("<mark>marked text</mark>");
    }

    [TestMethod]
    public void AddFluidMarkdownFilters_CanBeChainedWithOtherConfigurations()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();

        // Act
        var result = options.AddFluidMarkdownFilters().AddFluidMarkdownFilters(); // Can be called multiple times

        // Assert
        result.Should().BeSameAs(options);
        MvcTestHelper.HasFilter(options.TemplateOptions, "markdown").Should().BeTrue();
    }

    [TestMethod]
    public void AddFluidMarkdownFilters_WithNullConfiguration_UsesDefaultOptions()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();

        // Act
        options.AddFluidMarkdownFilters(null);

        // Assert
        MvcTestHelper.HasFilter(options.TemplateOptions, "markdown").Should().BeTrue();
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_IHtmlContentValueConverter_WorksWithHtmlString()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);
        var htmlString = new HtmlString("<strong>pre-encoded</strong>");
        context.SetValue("html", htmlString);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync("{{ html }}", context);

        // Assert
        output.Should().Contain("<strong>pre-encoded</strong>");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_ValueConverter_PreservesHtmlEncoding()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);
        var htmlString = new HtmlString("<p>&lt;script&gt;</p>");
        context.SetValue("html", htmlString);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync("{{ html }}", context);

        // Assert
        output
            .Should()
            .Contain("&lt;script&gt;", "HTML encoding should be preserved, not double-encoded");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_ReturnsHtmlStringFromFilter()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);

        // Parse template and get the result
        var template = MvcTestHelper.ParseTemplate("{{ '**test**' | markdown }}");
        var output = await template.RenderAsync(context);

        // Assert - The output should contain unescaped HTML
        output.Should().Contain("<strong>test</strong>");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_HandlesEmptyMarkdown()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync("{{ '' | markdown }}", context);

        // Assert
        output.Should().BeEmpty();
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_HandlesNullInput()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);
        context.SetValue("content", null!);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync("{{ content | markdown }}", context);

        // Assert
        output.Should().BeEmpty();
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_WithMultipleMarkdownBlocks_RendersAll()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);
        context.SetValue("title", "# Title");
        context.SetValue("content", "**Bold content**");

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            "{{ title | markdown }}{{ content | markdown }}",
            context
        );

        // Assert
        output.Should().Contain("<h1").And.Contain("<strong>Bold content</strong>");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_SupportsLinks()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            "{{ '[Link](https://example.com)' | markdown }}",
            context
        );

        // Assert
        output.Should().Contain("<a").And.Contain("href=\"https://example.com\"");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_SupportsCodeBlocks()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            "{{ '`inline code`' | markdown }}",
            context
        );

        // Assert
        output.Should().Contain("<code>inline code</code>");
    }

    [TestMethod]
    public async Task AddFluidMarkdownFilters_SupportsLists()
    {
        // Arrange
        var options = MvcTestHelper.CreateFluidViewEngineOptions();
        options.AddFluidMarkdownFilters();
        var context = MvcTestHelper.CreateContext(options.TemplateOptions);
        var markdown = "- Item 1\n- Item 2";

        // Act
        var output = await MvcTestHelper.RenderTemplateAsync(
            $"{{{{ '{markdown}' | markdown }}}}",
            context
        );

        // Assert
        output
            .Should()
            .Contain("<ul>")
            .And.Contain("<li>Item 1</li>")
            .And.Contain("<li>Item 2</li>");
    }
}
