using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Infrastructure.Notifications;
using BookMyHall.Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Moq;


namespace BookMyHall.Infrastructure.Tests.Notifications;

public sealed class PasswordResetRequestedEventHandlerTests : IDisposable
{
    private const string UserName = "Rakesh Yadav";
    private const string Email = "rakesh@example.com";
    private const string ResetToken = "reset-token-123";
    private const string BaseUrl = "https://bookmyhall.com";

    private readonly Mock<IEmailTemplateService> _templateServiceMock = new();
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<IWebHostEnvironment> _environmentMock = new();

    private readonly string _webRootPath;
    private readonly string _logoPath;

    public PasswordResetRequestedEventHandlerTests()
    {
        _webRootPath = Path.Combine(
            Path.GetTempPath(),
            $"PasswordResetTests_{Guid.NewGuid():N}");

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

    [Fact]
    public async Task Handle_ShouldRenderTemplateAndSendEmail()
    {
        // Arrange
        const string renderedHtml =
            "<html><body>Password reset email.</body></html>";

        _templateServiceMock
            .Setup(x => x.RenderAsync(
                "PasswordReset",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(renderedHtml);

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        _templateServiceMock.Verify(
            x => x.RenderAsync(
                "PasswordReset",
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
    public async Task Handle_ShouldRenderTemplateWithExpectedValues()
    {
        // Arrange
        IReadOnlyDictionary<string, string>? templateModel = null;

        _templateServiceMock
            .Setup(x => x.RenderAsync(
                "PasswordReset",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .Callback<
                string,
                IReadOnlyDictionary<string, string>,
                CancellationToken>(
                (_, model, _) => templateModel = model)
            .ReturnsAsync("<p>Password reset.</p>");

        var sut = CreateSut();

        // Act
        await sut.Handle(CreateNotification(),CancellationToken.None);

        // Assert
        Assert.NotNull(templateModel);
        Assert.Equal(UserName,templateModel["UserName"]);
        Assert.Equal(DateTime.UtcNow.Year.ToString(),templateModel["CurrentYear"]);
        Assert.Contains("reset-password",templateModel["ResetLink"]);
    }

    [Fact]
    public async Task Handle_ShouldGenerateResetLinkWithEmailAndToken()
    {
        // Arrange
        IReadOnlyDictionary<string, string>? templateModel = null;

        _templateServiceMock
            .Setup(x => x.RenderAsync(
                "PasswordReset",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .Callback<
                string,
                IReadOnlyDictionary<string, string>,
                CancellationToken>(
                (_, model, _) => templateModel = model)
            .ReturnsAsync("<p>Password reset.</p>");

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(templateModel);

        var resetLink = templateModel["ResetLink"];

        Assert.Contains(
            $"{BaseUrl}/reset-password",
            resetLink);

        Assert.Contains(
            $"email={Uri.EscapeDataString(Email)}",
            resetLink);

        Assert.Contains(
            $"token={Uri.EscapeDataString(ResetToken)}",
            resetLink);
    }

    [Fact]
    public async Task Handle_ShouldCreateEmailWithExpectedContent()
    {
        // Arrange
        const string renderedHtml =
            "<p>Reset your BookMyHall password.</p>";

        EmailMessage? sentEmail = null;

        _templateServiceMock
            .Setup(x => x.RenderAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(renderedHtml);

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>(
                (email, _) => sentEmail = email)
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        var email = Assert.IsType<EmailMessage>(sentEmail);

        Assert.Equal(
            Email,
            email.To);

        Assert.Equal(
            "Reset your BookMyHall password",
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
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("<p>Password reset.</p>");

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailMessage, CancellationToken>(
                (email, _) => sentEmail = email)
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.Handle(
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

        var sut = new PasswordResetRequestedEventHandler(
            _templateServiceMock.Object,
            _emailSenderMock.Object,
            Microsoft.Extensions.Options.Options.Create(
                new FrontendOptions
                {
                    BaseUrl = BaseUrl
                }),
            environmentMock.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<FileNotFoundException>(
                () => sut.Handle(
                    CreateNotification(),
                    CancellationToken.None));

        // Assert
        Assert.Equal("Email logo not found",exception.Message);
        Assert.EndsWith(Path.Combine("images", "logo.png"),exception.FileName);

        _templateServiceMock.Verify(
            x => x.RenderAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyDictionary<string, string>>(),
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
                It.IsAny<string>(),
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                cancellationToken))
            .ReturnsAsync("<p>Password reset.</p>");

        _emailSenderMock
            .Setup(x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                cancellationToken))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            cancellationToken);

        // Assert
        _templateServiceMock.Verify(
            x => x.RenderAsync(
                "PasswordReset",
                It.IsAny<IReadOnlyDictionary<string, string>>(),
                cancellationToken),
            Times.Once);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<EmailMessage>(),
                cancellationToken),
            Times.Once);
    }

    private PasswordResetRequestedEventHandler CreateSut()
    {
        return new PasswordResetRequestedEventHandler(
            _templateServiceMock.Object,
            _emailSenderMock.Object,
            Microsoft.Extensions.Options.Options.Create(
                new FrontendOptions
                {
                    BaseUrl = BaseUrl
                }),
            _environmentMock.Object);
    }

    private static PasswordResetRequestedEvent CreateNotification()
    {
        return new PasswordResetRequestedEvent(Guid.NewGuid(),UserName,Email,ResetToken);
    }

    public void Dispose()
    {
        if (Directory.Exists(_webRootPath))
        {
            Directory.Delete(_webRootPath,recursive: true);
        }
    }
}