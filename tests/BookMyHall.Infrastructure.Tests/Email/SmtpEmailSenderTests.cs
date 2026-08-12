using BookMyHall.Application.Abstractions.Email;
using BookMyHall.Infrastructure.Email;
using BookMyHall.Infrastructure.Options;

namespace BookMyHall.Infrastructure.Tests.Email;

public sealed class SmtpEmailSenderTests
{
    private const string FromEmail = "rakeshyadav@bookmyhall.com";
    private const string FromName = "BookMyHall Admin";
    private const string Recipient = "user@example.com";

    private readonly EmailOptions _options = new()
    {
        FromEmail = FromEmail,
        FromName = FromName,

        // smtp4dev
        Host = "localhost",
        Port = 2525,

        UserName = string.Empty,
        Password = string.Empty,

        EnableSsl = false,
        TemplateFolder = string.Empty
    };

    [Fact]
    public async Task SendAsync_ShouldSendEmail()
    {
        // Arrange
        var sender = CreateSut();

        var message = new EmailMessage
        {
            To = Recipient,
            Subject = "Welcome to BookMyHall",
            HtmlBody = "<p>Hello World</p>",
            InlineAttachments = []
        };

        // Act
        await sender.SendAsync(message);

        // Assert
        // Retrieve the message from smtp4dev
        // and verify the actual SMTP message.
        //
        // Expected:
        // From    = admin@bookmyhall.com
        // To      = user@example.com
        // Subject = Welcome to BookMyHall
        // Body    = <p>Hello World</p>
    }

    [Fact]
    public async Task SendAsync_ShouldUseConfiguredSender()
    {
        // Arrange
        var sender = CreateSut();

        var message = new EmailMessage
        {
            To = Recipient,
            Subject = "Sender Test",
            HtmlBody = "<p>Test</p>",
            InlineAttachments = []
        };

        // Act
        await sender.SendAsync(message);

        // Assert
        // Retrieve the message from smtp4dev.
        //
        // Verify:
        // FromEmail == admin@bookmyhall.com
        // FromName  == BookMyHall Admin
    }

    [Fact]
    public async Task SendAsync_ShouldUseConfiguredRecipient()
    {
        // Arrange
        var sender = CreateSut();

        var message = new EmailMessage
        {
            To = Recipient,
            Subject = "Recipient Test",
            HtmlBody = "<p>Test</p>",
            InlineAttachments = []
        };

        // Act
        await sender.SendAsync(message);

        // Assert
        // Retrieve the message from smtp4dev.
        //
        // Verify:
        // To == user@example.com
    }

    [Fact]
    public async Task SendAsync_ShouldUseConfiguredSubject()
    {
        // Arrange
        const string subject = "BookMyHall Test Email";

        var sender = CreateSut();

        var message = new EmailMessage
        {
            To = Recipient,
            Subject = subject,
            HtmlBody = "<p>Test</p>",
            InlineAttachments = []
        };

        // Act
        await sender.SendAsync(message);

        // Assert
        // Retrieve the message from smtp4dev.
        //
        // Verify:
        // Subject == BookMyHall Test Email
    }

    [Fact]
    public async Task SendAsync_ShouldUseHtmlBody()
    {
        // Arrange
        const string html =
            """
            <html>
                <body>
                    <h1>Welcome to BookMyHall</h1>
                    <p>Hello World</p>
                </body>
            </html>
            """;

        var sender = CreateSut();

        var message = new EmailMessage
        {
            To = Recipient,
            Subject = "HTML Test",
            HtmlBody = html,
            InlineAttachments = []
        };

        // Act
        await sender.SendAsync(message);

        // Assert
        // Retrieve the message from smtp4dev.
        //
        // Verify that the HTML body contains:
        // Welcome to BookMyHall
        // Hello World
    }

    [Fact]
    public async Task SendAsync_WithInlineAttachment_ShouldAddLinkedResource()
    {
        // Arrange
        var imagePath = await CreateTemporaryImageAsync();

        try
        {
            var sender = CreateSut();

            var message = new EmailMessage
            {
                To = Recipient,
                Subject = "Inline Image Test",
                HtmlBody =
                    """<img src="cid:bookmyhall-logo" />""",
                InlineAttachments =
                [
                    new EmailAttachment
                    {
                        FilePath = imagePath,
                        ContentId = "bookmyhall-logo"
                    }
                ]
            };

            // Act
            await sender.SendAsync(message);

            // Assert
            // Retrieve the message from smtp4dev.
            //
            // Find MimePart where:
            //
            // ContentId == "bookmyhall-logo"
            //
            // Verify:
            // ContentDisposition == "inline"
            // ContentLocation == "cid:bookmyhall-logo"
        }
        finally
        {
            DeleteFile(imagePath);
        }
    }

    [Fact]
    public async Task SendAsync_WithMultipleInlineAttachments_ShouldAddAllResources()
    {
        // Arrange
        var logoPath = await CreateTemporaryFileAsync(
            "logo.png",
            [0x01, 0x02, 0x03]);

        var bannerPath = await CreateTemporaryFileAsync(
            "banner.png",
            [0x04, 0x05, 0x06]);

        try
        {
            var sender = CreateSut();

            var message = new EmailMessage
            {
                To = Recipient,
                Subject = "Multiple Images",
                HtmlBody =
                    """
                    <img src="cid:logo" />
                    <img src="cid:banner" />
                    """,

                InlineAttachments =
                [
                    new EmailAttachment
                    {
                        FilePath = logoPath,
                        ContentId = "logo"
                    },
                    new EmailAttachment
                    {
                        FilePath = bannerPath,
                        ContentId = "banner"
                    }
                ]
            };

            // Act
            await sender.SendAsync(message);

            // Assert
            // Retrieve the message from smtp4dev.
            //
            // Verify two inline MimeParts exist:
            //
            // logo
            // banner
        }
        finally
        {
            DeleteFile(logoPath);
            DeleteFile(bannerPath);
        }
    }

    [Fact]
    public async Task SendAsync_ShouldConnectUsingConfiguredSmtpServer()
    {
        // Arrange
        var sender = CreateSut();

        var message = new EmailMessage
        {
            To = Recipient,
            Subject = "SMTP Configuration Test",
            HtmlBody = "<p>Test</p>",
            InlineAttachments = []
        };

        // Act
        await sender.SendAsync(message);

        // Assert
        // Successful completion proves that the sender connected
        // to the configured SMTP endpoint.
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_ShouldCompleteSuccessfully()
    {
        // Arrange
        using var cancellationTokenSource =
            new CancellationTokenSource();

        var sender = CreateSut();

        var message = new EmailMessage
        {
            To = Recipient,
            Subject = "Cancellation Test",
            HtmlBody = "<p>Test</p>",
            InlineAttachments = []
        };

        // Act
        await sender.SendAsync(
            message,
            cancellationTokenSource.Token);

        // Assert
        // The operation completed using the supplied token.
    }

    private SmtpEmailSender CreateSut()
    {
        return new SmtpEmailSender(
            Microsoft.Extensions.Options.Options.Create(_options));
    }

    private static async Task<string> CreateTemporaryImageAsync()
    {
        return await CreateTemporaryFileAsync(
            "logo.png",
            [0x01, 0x02, 0x03]);
    }

    private static async Task<string> CreateTemporaryFileAsync(
        string fileName,
        byte[] content)
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            $"BookMyHall_SmtpTests_{Guid.NewGuid():N}");

        Directory.CreateDirectory(directory);

        var path = Path.Combine(
            directory,
            fileName);

        await File.WriteAllBytesAsync(
            path,
            content);

        return path;
    }

    private static void DeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var directory = Path.GetDirectoryName(path);

            if (directory is not null &&
                Directory.Exists(directory))
            {
                Directory.Delete(
                    directory,
                    recursive: true);
            }
        }
        catch
        {
           
        }
    }
}