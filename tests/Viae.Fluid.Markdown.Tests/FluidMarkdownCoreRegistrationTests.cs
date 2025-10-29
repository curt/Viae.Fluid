// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using Viae.Fluid.Markdown.Tests.TestHelpers;

namespace Viae.Fluid.Markdown.Tests;

[TestClass]
public sealed class FluidMarkdownCoreRegistrationTests
{
    [TestMethod]
    public void CreateCore_WithoutOptions_ReturnsFilterAndOptions()
    {
        // Act
        var (filter, options) = FluidMarkdownCoreRegistration.CreateCore();

        // Assert
        filter.Should().NotBeNull();
        options.Should().NotBeNull();
    }

    [TestMethod]
    public void CreateCore_WithoutOptions_RegistersMarkdownFilter()
    {
        // Act
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();

        // Assert
        FluidTestHelper.HasFilter(options, "markdown").Should().BeTrue();
    }

    [TestMethod]
    public void CreateCore_WithoutOptions_RegistersMarkdownifyFilter()
    {
        // Act
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();

        // Assert
        FluidTestHelper.HasFilter(options, "markdownify").Should().BeTrue();
    }

    [TestMethod]
    public async Task CreateCore_WithoutOptions_MarkdownFilterWorks()
    {
        // Arrange
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();
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
    public async Task CreateCore_WithoutOptions_MarkdownifyFilterWorks()
    {
        // Arrange
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();
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
    public void CreateCore_WithCustomOptions_UsesCustomConfiguration()
    {
        // Arrange
        var customOptions = new MarkdownFilterOptions
        {
            ConfigurePipeline = b => b, // No extensions
        };

        // Act
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore(customOptions);

        // Assert
        options.Should().NotBeNull();
        FluidTestHelper.HasFilter(options, "markdown").Should().BeTrue();
    }

    [TestMethod]
    public async Task CreateCore_WithCustomOptions_UsesCustomPipeline()
    {
        // Arrange
        var customOptions = new MarkdownFilterOptions
        {
            ConfigurePipeline = b => b, // No extensions, so strikethrough won't work
        };

        // Act
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore(customOptions);
        var context = FluidTestHelper.CreateContext(options);
        var output = await FluidTestHelper.RenderTemplateAsync(
            "{{ '~~strikethrough~~' | markdown }}",
            context
        );

        // Assert - Without extensions, strikethrough won't be processed
        output.Should().NotContain("<del>");
    }

    [TestMethod]
    public async Task CreateCore_WithAdvancedExtensions_SupportsStrikethrough()
    {
        // Arrange
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();
        var context = FluidTestHelper.CreateContext(options);

        // Act
        var output = await FluidTestHelper.RenderTemplateAsync(
            "{{ '~~strikethrough~~' | markdown }}",
            context
        );

        // Assert
        output.Should().Contain("<del>strikethrough</del>");
    }

    [TestMethod]
    public async Task CreateCore_WithAdvancedExtensions_SupportsTables()
    {
        // Arrange
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "| Header |\n|--------|\n| Cell   |";

        // Act
        var output = await FluidTestHelper.RenderTemplateAsync(
            $"{{{{ '{markdown}' | markdown }}}}",
            context
        );

        // Assert
        output.Should().Contain("<table>");
    }

    [TestMethod]
    public async Task CreateCore_WithAdvancedExtensions_SupportsTaskLists()
    {
        // Arrange
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "- [x] Checked";

        // Act
        var output = await FluidTestHelper.RenderTemplateAsync(
            $"{{{{ '{markdown}' | markdown }}}}",
            context
        );

        // Assert
        output.Should().Contain("<input").And.Contain("type=\"checkbox\"");
    }

    [TestMethod]
    public void CreateCore_ReturnedFilter_IsNotNull()
    {
        // Act
        var (filter, _) = FluidMarkdownCoreRegistration.CreateCore();

        // Assert
        filter.Should().NotBeNull().And.BeOfType<MarkdownFilter>();
    }

    [TestMethod]
    public void CreateCore_ReturnedOptions_IsNotNull()
    {
        // Act
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();

        // Assert
        options.Should().NotBeNull().And.BeOfType<TemplateOptions>();
    }

    [TestMethod]
    public async Task CreateCore_MultipleCalls_CreateIndependentInstances()
    {
        // Arrange & Act
        var (filter1, options1) = FluidMarkdownCoreRegistration.CreateCore();
        var (filter2, options2) = FluidMarkdownCoreRegistration.CreateCore();

        // Assert
        filter1.Should().NotBeSameAs(filter2);
        options1.Should().NotBeSameAs(options2);
    }

    [TestMethod]
    public async Task CreateCore_WithComplexMarkdown_RendersCorrectly()
    {
        // Arrange
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();
        var context = FluidTestHelper.CreateContext(options);
        var markdown = "# Title\n\n**Bold** and *italic* with [link](https://example.com)";

        // Act
        var output = await FluidTestHelper.RenderTemplateAsync(
            $"{{{{ '{markdown}' | markdown }}}}",
            context
        );

        // Assert
        output
            .Should()
            .Contain("<h1")
            .And.Contain("<strong>Bold</strong>")
            .And.Contain("<em>italic</em>")
            .And.Contain("<a")
            .And.Contain("href=\"https://example.com\"");
    }

    [TestMethod]
    public async Task CreateCore_WithEmptyString_ReturnsEmptyString()
    {
        // Arrange
        var (_, options) = FluidMarkdownCoreRegistration.CreateCore();
        var context = FluidTestHelper.CreateContext(options);

        // Act
        var output = await FluidTestHelper.RenderTemplateAsync("{{ '' | markdown }}", context);

        // Assert
        output.Should().BeEmpty();
    }
}
