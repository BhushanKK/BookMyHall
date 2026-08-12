using BookMyHall.Infrastructure.Options;

using FluentAssertions;

namespace BookMyHall.Tests.Infrastructure.Options;

public sealed class FrontendOptionsTests
{
    [Fact]
    public void SectionName_ShouldBeFrontend()
    {
        // Act
        var sectionName = FrontendOptions.SectionName;

        // Assert
        sectionName.Should().Be("Frontend");
    }

    [Fact]
    public void BaseUrl_ShouldHaveEmptyDefaultValue()
    {
        // Arrange
        var options = new FrontendOptions();

        // Act
        var baseUrl = options.BaseUrl;

        // Assert
        baseUrl.Should().BeEmpty();
    }

    [Fact]
    public void BaseUrl_ShouldSetAndGetValue()
    {
        // Arrange
        var options = new FrontendOptions();
        const string expectedBaseUrl = "https://localhost:3000";

        // Act
        options.BaseUrl = expectedBaseUrl;

        // Assert
        options.BaseUrl.Should().Be(expectedBaseUrl);
    }
}