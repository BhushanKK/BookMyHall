using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Infrastructure.Notifications;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace BookMyHall.Infrastructure.Tests.Notifications;
public sealed class WelcomeEmailEventHandlerTests
{
    private const string UserName = "Rakesh Yadav";
    private const string Email = "rakesh@example.com";
    private const string TemplateHtml ="<html><body>Welcome Rakesh Yadav</body></html>";

    private readonly Guid _userId = Guid.NewGuid();

    private readonly Mock<IEmailTemplateService>
        _emailTemplateServiceMock = new();

    private readonly Mock<IEmailSender>
        _emailSenderMock = new();

    private readonly Mock<IWebHostEnvironment>
        _environmentMock = new();

    private readonly string _webRootPath;

    public WelcomeEmailEventHandlerTests()
    {
        _webRootPath = Path.Combine(
            Path.GetTempPath(),
            $"BookMyHall_WelcomeEmailTests_{Guid.NewGuid():N}");

        Directory.CreateDirectory(
            Path.Combine(_webRootPath, "images"));

        _environmentMock
            .SetupGet(x => x.WebRootPath)
            .Returns(_webRootPath);

        _emailTemplateServiceMock
            .Setup(x => x.RenderAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(TemplateHtml);

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_ShouldRenderWelcomeTemplate()
    {
        // Arrange
        var sut = CreateSut();
        var notification = CreateNotification();

        // Act
        await sut.Handle(notification,CancellationToken.None);

        // Assert
        _emailTemplateServiceMock.Verify(
            x => x.RenderAsync(
                "Welcome",It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassExpectedTemplateValues()
    {
        // Arrange
        var sut = CreateSut();
        var notification = CreateNotification();
        IReadOnlyDictionary<string, string>? placeholders = null;

        _emailTemplateServiceMock
            .Setup(x => x.RenderAsync("Welcome",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .Callback<
                string,
                IReadOnlyDictionary<string, string>,
                CancellationToken>(
                (_, values, _) =>
                {
                    placeholders = values;
                })
            .ReturnsAsync(TemplateHtml);

        // Act
        await sut.Handle(notification,CancellationToken.None);

        // Assert
        Assert.NotNull(placeholders);
        Assert.Equal(UserName,placeholders["UserName"]);

        Assert.Equal(DateTime.UtcNow.Year.ToString(),placeholders["CurrentYear"]);
    }

    [Fact]
    public async Task Handle_ShouldSendEmailToRegisteredUser()
    {
        // Arrange
        var sut = CreateSut();
        var notification = CreateNotification();

        EmailMessage? sentEmail = null;

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>(
                (email, _) =>
                {
                    sentEmail = email;
                })
            .Returns(Task.CompletedTask);

        // Act
        await sut.Handle(notification,CancellationToken.None);

        // Assert
        Assert.NotNull(sentEmail);
        Assert.Equal(Email,sentEmail.To);
        Assert.Equal("Welcome to BookMyHall 🎉",sentEmail.Subject);
        Assert.Equal(TemplateHtml,sentEmail.HtmlBody);
    }

    [Fact]
    public async Task Handle_ShouldAddBookMyHallLogoAsInlineAttachment()
    {
        // Arrange
        var sut = CreateSut();
        var notification = CreateNotification();
        EmailMessage? sentEmail = null;

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>(
                (email, _) =>
                {
                    sentEmail = email;
                })
            .Returns(Task.CompletedTask);

        // Act
        await sut.Handle(
            notification,
            CancellationToken.None);

        // Assert
        Assert.NotNull(sentEmail);

        var attachment = Assert.Single(
            sentEmail.InlineAttachments);

        Assert.Equal(
            "bookmyhall-logo",
            attachment.ContentId);

        Assert.Equal(
            Path.Combine(
                _webRootPath,
                "images",
                "logo.png"),
            attachment.FilePath);
    }

    [Fact]
    public async Task Handle_ShouldUseRenderedHtmlAsEmailBody()
    {
        // Arrange
        const string html ="<h1>Welcome to BookMyHall</h1>";

        _emailTemplateServiceMock
            .Setup(x => x.RenderAsync("Welcome",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(html);

        EmailMessage? sentEmail = null;

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>(
                (email, _) =>
                {
                    sentEmail = email;
                })
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(sentEmail);
        Assert.Equal(html, sentEmail.HtmlBody);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToDependencies()
    {
        // Arrange
        var sut = CreateSut();
        var notification = CreateNotification();

        using var cancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            cancellationTokenSource.Token;

        // Act
        await sut.Handle(
            notification,
            cancellationToken);

        // Assert
        _emailTemplateServiceMock.Verify(
            x => x.RenderAsync(
                "Welcome",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                cancellationToken),
            Times.Once);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRenderTemplateBeforeSendingEmail()
    {
        // Arrange
        var sequence = new MockSequence();

        _emailTemplateServiceMock
            .InSequence(sequence)
            .Setup(x => x.RenderAsync(
                "Welcome",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(TemplateHtml);

        _emailSenderMock
            .InSequence(sequence)
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        _emailTemplateServiceMock.Verify(
            x => x.RenderAsync(
                "Welcome",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTemplateRenderingFails_ShouldNotSendEmail()
    {
        // Arrange
        var exception = new InvalidOperationException(
            "Template rendering failed.");

        _emailTemplateServiceMock
            .Setup(x => x.RenderAsync(
                "Welcome",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();

        // Act
        var act = () => sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmailSendingFails_ShouldPropagateException()
    {
        // Arrange
        var exception = new InvalidOperationException(
            "Email sending failed.");

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();

        // Act
        var act = () => sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);

        _emailTemplateServiceMock.Verify(
            x => x.RenderAsync("Welcome",It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()),Times.Once);
    }

    private WelcomeEmailEventHandler CreateSut()
    {
        return new WelcomeEmailEventHandler(
            _emailTemplateServiceMock.Object,
            _emailSenderMock.Object,
            _environmentMock.Object);
    }

    private UserRegisteredEvent CreateNotification()
    {
        return new UserRegisteredEvent(_userId,UserName,Email);
    }
}