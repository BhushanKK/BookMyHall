using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Infrastructure.Notifications;
using BookMyHall.Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;


namespace BookMyHall.Infrastructure.Tests.Notifications;

public sealed class EmailVerificationRequestedEventHandlerTests : IDisposable
{
    private const string BaseUrl = "https://bookmyhall.com";
    private const string UserName = "Rakesh Yadav";
    private const string Email = "rakesh@example.com";
    private const string VerificationToken = "verification-token-123";

    private readonly Mock<IEmailTemplateService> _templateServiceMock = new();
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<IWebHostEnvironment> _environmentMock = new();

    private readonly string _webRootPath;
    private readonly string _logoPath;

    public EmailVerificationRequestedEventHandlerTests()
    {
        _webRootPath = Path.Combine(Path.GetTempPath(),
            $"BookMyHallTests_{Guid.NewGuid():N}");

        _logoPath = Path.Combine(
            _webRootPath,
            "images",
            "logo.png");

        Directory.CreateDirectory(
            Path.GetDirectoryName(_logoPath)!);

        File.WriteAllBytes(
            _logoPath,
            [0x01, 0x02, 0x03]);

        _environmentMock
            .SetupGet(x => x.WebRootPath)
            .Returns(_webRootPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_webRootPath))
        {
            Directory.Delete(
                _webRootPath,
                recursive: true);
        }
    }

    [Fact]
    public async Task Handle_ShouldRenderTemplateAndSendEmail()
    {
        // Arrange
        const string renderedHtml ="<html><body>Verify your email</body></html>";

        _templateServiceMock.Setup(x => x.RenderAsync("VerifyEmail",It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(renderedHtml);

        var handler = CreateHandler();
        var notification = CreateNotification();

        // Act
        await handler.Handle(notification,CancellationToken.None);

        // Assert
        _templateServiceMock.Verify(
            x => x.RenderAsync("VerifyEmail",It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()),Times.Once);

        _emailSenderMock.Verify(x => x.SendAsync(
                It.IsAny<EmailMessage>(),It.IsAny<CancellationToken>()),Times.Once);
    }

    
    [Fact]
    public async Task Handle_ShouldCreateEmailWithExpectedContent()
    {
        // Arrange
        const string renderedHtml =
            "<p>Verify your email</p>";

        EmailMessage? sentEmail = null;

        _templateServiceMock
            .Setup(x => x.RenderAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(renderedHtml);

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>(
                (email, _) => sentEmail = email)
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        var email = Assert.IsType<EmailMessage>(sentEmail);

        Assert.Equal(Email, email.To);

        Assert.Equal(
            "Verify your BookMyHall email address",
            email.Subject);

        Assert.Equal(
            renderedHtml,
            email.HtmlBody);
    }

    [Fact]
    public async Task Handle_ShouldAddLogoAsInlineAttachment()
    {
        // Arrange
        EmailMessage? sentEmail = null;

        _templateServiceMock
            .Setup(x => x.RenderAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("<p>Verify</p>");

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>(
                (email, _) => sentEmail = email)
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        var email = Assert.IsType<EmailMessage>(sentEmail);

        var attachment = Assert.Single(
            email.InlineAttachments);

        Assert.Equal(
            _logoPath,
            attachment.FilePath);

        Assert.Equal(
            "bookmyhall-logo",
            attachment.ContentId);
    }

    
    [Fact]
    public async Task Handle_WhenLogoDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var missingWebRoot = Path.Combine(
            Path.GetTempPath(),
            $"MissingWebRoot_{Guid.NewGuid():N}");

        var environmentMock =
            new Mock<IWebHostEnvironment>();

        environmentMock
            .SetupGet(x => x.WebRootPath)
            .Returns(missingWebRoot);

        var handler = CreateHandler(
            environment: environmentMock);

        // Act
        var exception =
            await Assert.ThrowsAsync<FileNotFoundException>(
                () => handler.Handle(
                    CreateNotification(),
                    CancellationToken.None));

        // Assert
        Assert.Equal(
            "Email logo not found.",
            exception.Message);

        Assert.EndsWith(
            Path.Combine("images", "logo.png"),
            exception.FileName);

        _templateServiceMock.Verify(
            x => x.RenderAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToDependencies()
    {
        // Arrange
        using var cancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            cancellationTokenSource.Token;

        _templateServiceMock
            .Setup(x => x.RenderAsync(
                "VerifyEmail",
                It.IsAny<Dictionary<string, string>>(),
                cancellationToken))
            .ReturnsAsync("<p>Verify</p>");

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                cancellationToken))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            CreateNotification(),
            cancellationToken);

        // Assert
        _templateServiceMock.Verify(
            x => x.RenderAsync(
                "VerifyEmail",
                It.IsAny<Dictionary<string, string>>(),
                cancellationToken),
            Times.Once);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                cancellationToken),
            Times.Once);
    }

    private EmailVerificationRequestedEventHandler CreateHandler(
        IOptions<FrontendOptions>? frontendOptions = null,
        Mock<IWebHostEnvironment>? environment = null)
    {
        return new EmailVerificationRequestedEventHandler(
            _templateServiceMock.Object,
            _emailSenderMock.Object,
            frontendOptions ??
            Microsoft.Extensions.Options.Options.Create(
                new FrontendOptions
                {
                    BaseUrl = BaseUrl
                }),
            (environment ?? _environmentMock).Object);
    }

    private static EmailVerificationRequestedEvent CreateNotification()
    {
        return new EmailVerificationRequestedEvent(
            Guid.NewGuid(),
            UserName,
            Email,
            VerificationToken);
    }
}