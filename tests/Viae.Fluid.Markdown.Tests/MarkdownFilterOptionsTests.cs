// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

namespace Viae.Fluid.Markdown.Tests;

[TestClass]
public sealed class MarkdownFilterOptionsTests
{
    [TestMethod]
    public void Constructor_CreatesDefaultOptions()
    {
        // Act
        var options = new MarkdownFilterOptions();

        // Assert
        options.Should().NotBeNull();
        options.ConfigurePipeline.Should().NotBeNull();
    }

    [TestMethod]
    public void ConfigurePipeline_DefaultConfiguration_UsesAdvancedExtensions()
    {
        // Arrange
        var options = new MarkdownFilterOptions();
        var builder = new MarkdownPipelineBuilder();

        // Act
        var configuredBuilder = options.ConfigurePipeline(builder);
        var pipeline = configuredBuilder.Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Assert - Test that advanced extensions work (strikethrough is part of advanced)
        var result = renderer.ToHtml("~~strikethrough~~");
        result.Should().Contain("<del>strikethrough</del>");
    }

    [TestMethod]
    public void ConfigurePipeline_DefaultConfiguration_SupportsTableExtension()
    {
        // Arrange
        var options = new MarkdownFilterOptions();
        var builder = new MarkdownPipelineBuilder();

        // Act
        var configuredBuilder = options.ConfigurePipeline(builder);
        var pipeline = configuredBuilder.Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var markdown = "| Header |\n|--------|\n| Cell   |";

        // Assert
        var result = renderer.ToHtml(markdown);
        result.Should().Contain("<table>");
    }

    [TestMethod]
    public void ConfigurePipeline_DefaultConfiguration_SupportsTaskListExtension()
    {
        // Arrange
        var options = new MarkdownFilterOptions();
        var builder = new MarkdownPipelineBuilder();

        // Act
        var configuredBuilder = options.ConfigurePipeline(builder);
        var pipeline = configuredBuilder.Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var markdown = "- [x] Checked";

        // Assert
        var result = renderer.ToHtml(markdown);
        result.Should().Contain("<input").And.Contain("type=\"checkbox\"");
    }

    [TestMethod]
    public void ConfigurePipeline_CustomConfiguration_CanBeOverridden()
    {
        // Arrange
        var options = new MarkdownFilterOptions
        {
            ConfigurePipeline = b => b, // No extensions
        };
        var builder = new MarkdownPipelineBuilder();

        // Act
        var configuredBuilder = options.ConfigurePipeline(builder);
        var pipeline = configuredBuilder.Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Assert - Without extensions, strikethrough won't work
        var result = renderer.ToHtml("~~strikethrough~~");
        result.Should().NotContain("<del>");
    }

    [TestMethod]
    public void ConfigurePipeline_WithCustomEmphasisExtension_WorksAsExpected()
    {
        // Arrange
        var options = new MarkdownFilterOptions { ConfigurePipeline = b => b.UseEmphasisExtras() };
        var builder = new MarkdownPipelineBuilder();

        // Act
        var configuredBuilder = options.ConfigurePipeline(builder);
        var pipeline = configuredBuilder.Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Assert - EmphasisExtras adds support for ==mark== syntax
        var result = renderer.ToHtml("==marked text==");
        result.Should().Contain("<mark>marked text</mark>");
    }

    [TestMethod]
    public void ConfigurePipeline_WithMultipleExtensions_AllExtensionsWork()
    {
        // Arrange
        var options = new MarkdownFilterOptions
        {
            ConfigurePipeline = b => b.UseEmphasisExtras().UsePipeTables(),
        };
        var builder = new MarkdownPipelineBuilder();

        // Act
        var configuredBuilder = options.ConfigurePipeline(builder);
        var pipeline = configuredBuilder.Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Assert - Both mark and tables should work
        var markResult = renderer.ToHtml("==marked==");
        markResult.Should().Contain("<mark>marked</mark>");

        var tableMarkdown = "| Col |\n|-----|\n| Val |";
        var tableResult = renderer.ToHtml(tableMarkdown);
        tableResult.Should().Contain("<table>");
    }

    [TestMethod]
    public void ConfigurePipeline_CanBeSetToNull_ThrowsException()
    {
        // Arrange
        var options = new MarkdownFilterOptions { ConfigurePipeline = null! };
        var builder = new MarkdownPipelineBuilder();

        // Act
        var act = () => options.ConfigurePipeline(builder);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [TestMethod]
    public void ConfigurePipeline_WithIdentityFunction_CreatesBasicPipeline()
    {
        // Arrange
        var options = new MarkdownFilterOptions { ConfigurePipeline = b => b };
        var builder = new MarkdownPipelineBuilder();

        // Act
        var configuredBuilder = options.ConfigurePipeline(builder);
        var pipeline = configuredBuilder.Build();
        var renderer = new MarkdigMarkdownRenderer(pipeline);

        // Assert - Basic markdown should still work
        var result = renderer.ToHtml("**bold**");
        result.Should().Contain("<strong>bold</strong>");
    }

    [TestMethod]
    public void ConfigurePipeline_WithChainedConfiguration_BuildsCorrectly()
    {
        // Arrange
        var options = new MarkdownFilterOptions
        {
            ConfigurePipeline = b =>
                b.UseAdvancedExtensions().UseEmphasisExtras().UsePipeTables().UseTaskLists(),
        };
        var builder = new MarkdownPipelineBuilder();

        // Act
        var configuredBuilder = options.ConfigurePipeline(builder);
        var pipeline = configuredBuilder.Build();

        // Assert - Should not throw and pipeline should be valid
        pipeline.Should().NotBeNull();
        var renderer = new MarkdigMarkdownRenderer(pipeline);
        var result = renderer.ToHtml("**test**");
        result.Should().Contain("<strong>test</strong>");
    }
}
