// Copyright © 2025 Curt Gilman and contributors
// SPDX-License-Identifier: MIT
// Viae Fluid: A set of custom filters for the Fluid template engine

using System.Globalization;
using System.Text.Encodings.Web;
using Viae.Fluid.Markdown.Mvc.Tests.TestHelpers;

namespace Viae.Fluid.Markdown.Mvc.Tests;

[TestClass]
public sealed class HtmlContentFluidValueTests
{
    [TestMethod]
    public void Constructor_WithHtmlString_CreatesInstance()
    {
        // Arrange
        var htmlString = new HtmlString("<strong>bold</strong>");

        // Act
        var value = new HtmlContentFluidValue(htmlString);

        // Assert
        value.Should().NotBeNull();
    }

    [TestMethod]
    public void Constructor_WithNull_CreatesInstanceWithEmptyString()
    {
        // Act
        var value = new HtmlContentFluidValue(null!);

        // Assert
        value.Should().NotBeNull();
        value.ToStringValue().Should().BeEmpty();
    }

    [TestMethod]
    public void Type_ReturnsString()
    {
        // Arrange
        var htmlString = new HtmlString("<p>test</p>");
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var type = value.Type;

        // Assert
        type.Should().Be(FluidValues.String);
    }

    [TestMethod]
    public void ToBooleanValue_WithNonEmptyHtml_ReturnsTrue()
    {
        // Arrange
        var htmlString = new HtmlString("<p>content</p>");
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var result = value.ToBooleanValue();

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void ToBooleanValue_WithEmptyHtml_ReturnsTrue()
    {
        // Arrange
        var htmlString = new HtmlString(string.Empty);
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var result = value.ToBooleanValue();

        // Assert
        result.Should().BeTrue("HTML content is always truthy even when empty");
    }

    [TestMethod]
    public void ToNumberValue_ReturnsZero()
    {
        // Arrange
        var htmlString = new HtmlString("<p>123</p>");
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var result = value.ToNumberValue();

        // Assert
        result.Should().Be(0m, "HTML content cannot be converted to a number");
    }

    [TestMethod]
    public void ToStringValue_WithSimpleHtml_ReturnsHtmlString()
    {
        // Arrange
        var htmlString = new HtmlString("<strong>bold</strong>");
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var result = value.ToStringValue();

        // Assert
        result.Should().Be("<strong>bold</strong>");
    }

    [TestMethod]
    public void ToStringValue_WithComplexHtml_ReturnsCompleteHtmlString()
    {
        // Arrange
        var html = "<div><p>Paragraph 1</p><p>Paragraph 2</p></div>";
        var htmlString = new HtmlString(html);
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var result = value.ToStringValue();

        // Assert
        result.Should().Be(html);
    }

    [TestMethod]
    public void ToStringValue_WithEmptyHtml_ReturnsEmptyString()
    {
        // Arrange
        var htmlString = new HtmlString(string.Empty);
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var result = value.ToStringValue();

        // Assert
        result.Should().BeEmpty();
    }

    [TestMethod]
    public void ToObjectValue_ReturnsUnderlyingIHtmlContent()
    {
        // Arrange
        var htmlString = new HtmlString("<em>italic</em>");
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var result = value.ToObjectValue();

        // Assert
        result.Should().BeSameAs(htmlString);
        result.Should().BeOfType<HtmlString>();
    }

    [TestMethod]
    public void Enumerate_ReturnsEmptyEnumerable()
    {
        // Arrange
        var htmlString = new HtmlString("<p>content</p>");
        var value = new HtmlContentFluidValue(htmlString);
        var context = MvcTestHelper.CreateContext(new TemplateOptions());

        // Act
        var result = value.Enumerate(context);

        // Assert
        result.Should().BeEmpty("HTML content is not iterable");
    }

    [TestMethod]
    public void Equals_WithSameReference_ReturnsTrue()
    {
        // Arrange
        var htmlString = new HtmlString("<p>test</p>");
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var result = value.Equals(value);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void Equals_WithIdenticalStringContent_ReturnsTrue()
    {
        // Arrange
        var htmlString1 = new HtmlString("<p>test</p>");
        var htmlString2 = new HtmlString("<p>test</p>");
        var value1 = new HtmlContentFluidValue(htmlString1);
        var value2 = new HtmlContentFluidValue(htmlString2);

        // Act
        var result = value1.Equals(value2);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void Equals_WithDifferentStringContent_ReturnsFalse()
    {
        // Arrange
        var htmlString1 = new HtmlString("<p>test1</p>");
        var htmlString2 = new HtmlString("<p>test2</p>");
        var value1 = new HtmlContentFluidValue(htmlString1);
        var value2 = new HtmlContentFluidValue(htmlString2);

        // Act
        var result = value1.Equals(value2);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var htmlString = new HtmlString("<p>test</p>");
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var result = value.Equals(null!);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void Equals_WithDifferentFluidValueType_ReturnsFalse()
    {
        // Arrange
        var htmlString = new HtmlString("<p>test</p>");
        var htmlValue = new HtmlContentFluidValue(htmlString);
        var stringValue = new StringValue("test");

        // Act
        var result = htmlValue.Equals(stringValue);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task WriteToAsync_WritesHtmlContentToWriter()
    {
        // Arrange
        var htmlString = new HtmlString("<strong>bold</strong>");
        var value = new HtmlContentFluidValue(htmlString);
        using var writer = new StringWriter();

        // Act
        await value.WriteToAsync(writer, HtmlEncoder.Default, CultureInfo.InvariantCulture);

        // Assert
        writer.ToString().Should().Be("<strong>bold</strong>");
    }

    [TestMethod]
    public async Task WriteToAsync_WithNullEncoder_UsesDefaultHtmlEncoder()
    {
        // Arrange
        var htmlString = new HtmlString("<em>italic</em>");
        var value = new HtmlContentFluidValue(htmlString);
        using var writer = new StringWriter();

        // Act
        await value.WriteToAsync(writer, null!, CultureInfo.InvariantCulture);

        // Assert
        writer.ToString().Should().Be("<em>italic</em>");
    }

    [TestMethod]
    public async Task WriteToAsync_WithComplexHtml_WritesCompleteHtml()
    {
        // Arrange
        var html = "<div class=\"container\"><p>Paragraph</p></div>";
        var htmlString = new HtmlString(html);
        var value = new HtmlContentFluidValue(htmlString);
        using var writer = new StringWriter();

        // Act
        await value.WriteToAsync(writer, HtmlEncoder.Default, CultureInfo.InvariantCulture);

        // Assert
        writer.ToString().Should().Be(html);
    }

    [TestMethod]
    public async Task WriteToAsync_WithEmptyHtml_WritesEmptyString()
    {
        // Arrange
        var htmlString = new HtmlString(string.Empty);
        var value = new HtmlContentFluidValue(htmlString);
        using var writer = new StringWriter();

        // Act
        await value.WriteToAsync(writer, HtmlEncoder.Default, CultureInfo.InvariantCulture);

        // Assert
        writer.ToString().Should().BeEmpty();
    }

    [TestMethod]
    public async Task WriteToAsync_PreservesHtmlEncoding()
    {
        // Arrange - HTML with special characters that should NOT be double-encoded
        var htmlString = new HtmlString("<p>&lt;script&gt;alert('xss')&lt;/script&gt;</p>");
        var value = new HtmlContentFluidValue(htmlString);
        using var writer = new StringWriter();

        // Act
        await value.WriteToAsync(writer, HtmlEncoder.Default, CultureInfo.InvariantCulture);

        // Assert - Should preserve the original HTML encoding
        writer
            .ToString()
            .Should()
            .Be(
                "<p>&lt;script&gt;alert('xss')&lt;/script&gt;</p>",
                "HTML encoding should be preserved"
            );
    }

    [TestMethod]
    public void ToStringValue_IsConsistentWithToObjectValue()
    {
        // Arrange
        var htmlString = new HtmlString("<code>test</code>");
        var value = new HtmlContentFluidValue(htmlString);

        // Act
        var stringValue = value.ToStringValue();
        var objectValue = value.ToObjectValue();
        var objectAsString = MvcTestHelper.HtmlContentToString((IHtmlContent)objectValue);

        // Assert
        stringValue.Should().Be(objectAsString, "string and object representations should match");
    }

    [TestMethod]
    public void Constructor_WithCustomIHtmlContent_WorksCorrectly()
    {
        // Arrange
        var customHtmlContent = new CustomHtmlContent("<custom>content</custom>");

        // Act
        var value = new HtmlContentFluidValue(customHtmlContent);

        // Assert
        value.ToStringValue().Should().Be("<custom>content</custom>");
    }

    // Helper class for testing custom IHtmlContent implementations
    private sealed class CustomHtmlContent(string content) : IHtmlContent
    {
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            writer.Write(content);
        }
    }
}
