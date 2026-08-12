using BookMyHall.Infrastructure.Email;
using BookMyHall.Infrastructure.Options;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace BookMyHall.Infrastructure.Tests.Email;

public sealed class EmailTemplateServiceTests : IDisposable
{
    private readonly string _testTemplateFolder;
    private readonly EmailOptions _emailOptions;
    private readonly Mock<IOptions<EmailOptions>> _optionsMock;

    public EmailTemplateServiceTests()
    {
     
        _testTemplateFolder = Path.Combine("TestTemplates", Guid.NewGuid().ToString("N"));
        var fullFolderPath = Path.Combine(AppContext.BaseDirectory, _testTemplateFolder);
        Directory.CreateDirectory(fullFolderPath);

        _emailOptions = new EmailOptions
        {
            TemplateFolder = _testTemplateFolder
        };

        _optionsMock = new Mock<IOptions<EmailOptions>>();
        _optionsMock.Setup(x => x.Value).Returns(_emailOptions);
    }

    public void Dispose()
    {
        
        var fullFolderPath = Path.Combine(AppContext.BaseDirectory, _testTemplateFolder);
        if (Directory.Exists(fullFolderPath))
        {
            Directory.Delete(fullFolderPath, recursive: true);
        }
    }

    [Fact]
    public async Task RenderAsync_WhenTemplateExists_ShouldReplacePlaceholdersAndReturnHtml()
    {
        // Arrange
        const string templateName = "WelcomeEmail";
        const string rawContent = "Hello {{Name}}, welcome to {{AppName}}! Your code is {{Code}}.";
        CreateTestTemplate(templateName, rawContent);

        var service = new EmailTemplateService(_optionsMock.Object);
        var placeholders = new Dictionary<string, string>
        {
            { "Name", "John Doe" },
            { "AppName", "BookMyHall" },
            { "Code", "123456" }
        };

        // Act
        var result = await service.RenderAsync(templateName, placeholders);

        // Assert
        result.Should().Be("Hello John Doe, welcome to BookMyHall! Your code is 123456.");
    }

    [Fact]
    public async Task RenderAsync_WhenTemplateDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var service = new EmailTemplateService(_optionsMock.Object);
        var placeholders = new Dictionary<string, string>();

        // Act
        Func<Task> act = async () => await service.RenderAsync("NonExistentTemplate", placeholders);

        // Assert
        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task RenderAsync_WhenPlaceholdersDictionaryIsEmpty_ShouldReturnUnmodifiedContent()
    {
        // Arrange
        const string templateName = "SimpleTemplate";
        const string rawContent = "<h1>Welcome {{Name}}</h1>";
        CreateTestTemplate(templateName, rawContent);

        var service = new EmailTemplateService(_optionsMock.Object);
        var placeholders = new Dictionary<string, string>();

        // Act
        var result = await service.RenderAsync(templateName, placeholders);

        // Assert
        result.Should().Be(rawContent);
    }

    [Fact]
    public async Task RenderAsync_WhenTemplateHasUnmatchedPlaceholders_ShouldLeaveUnmatchedPlaceholdersIntact()
    {
        // Arrange
        const string templateName = "PartialMatchTemplate";
        const string rawContent = "Hello {{Name}}, your role is {{Role}}.";
        CreateTestTemplate(templateName, rawContent);

        var service = new EmailTemplateService(_optionsMock.Object);
        var placeholders = new Dictionary<string, string>
        {
            { "Name", "Alice" }
            // "Role" is intentionally missing
        };

        // Act
        var result = await service.RenderAsync(templateName, placeholders);

        // Assert
        result.Should().Be("Hello Alice, your role is {{Role}}.");
    }

    private void CreateTestTemplate(string templateName, string content)
    {
        var filePath = Path.Combine( AppContext.BaseDirectory,_testTemplateFolder, $"{templateName}.html");

        File.WriteAllText(filePath, content);
    }
}