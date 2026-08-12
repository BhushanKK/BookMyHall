using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Features.Authentication.Events;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Infrastructure.Notifications;
using MediatR;
using Moq;

namespace BookMyHall.Infrastructure.Tests.Notifications;
public sealed class UserRegisteredEventHandlerTests
{
    private const string UserName = "Rakesh Yadav";
    private const string Email = "rakesh@example.com";
    private const string VerificationToken = "verification-token";
    private const string TokenHash = "hashed-verification-token";
    private readonly Guid _userId = Guid.NewGuid();

    private readonly Mock<IEmailVerificationTokenRepository>
        _emailVerificationTokenRepositoryMock = new();

    private readonly Mock<ITokenGenerator>
        _tokenGeneratorMock = new();

    private readonly Mock<ITokenHasher>
        _tokenHasherMock = new();

    private readonly Mock<IUnitOfWork>
        _unitOfWorkMock = new();

    private readonly Mock<IMediator>
        _mediatorMock = new();

    public UserRegisteredEventHandlerTests()
    {
        _tokenGeneratorMock
            .Setup(x => x.GenerateEmailVerificationToken())
            .Returns(VerificationToken);

        _tokenHasherMock
            .Setup(x => x.Hash(VerificationToken))
            .Returns(TokenHash);

        _emailVerificationTokenRepositoryMock
            .Setup(x => x.DeleteByUserIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _emailVerificationTokenRepositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<EmailVerificationToken>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mediatorMock
            .Setup(x => x.Publish(
                It.IsAny<EmailVerificationRequestedEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Handle_ShouldDeleteExistingVerificationTokens()
    {
        // Arrange
        var sut = CreateSut();
        var notification = CreateNotification();

        // Act
        await sut.Handle(notification,CancellationToken.None);

        // Assert
        _emailVerificationTokenRepositoryMock.Verify(
            x => x.DeleteByUserIdAsync(_userId,It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldGenerateVerificationToken()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        await sut.Handle(CreateNotification(),CancellationToken.None);

        // Assert
        _tokenGeneratorMock.Verify(x => x.GenerateEmailVerificationToken(),Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldHashGeneratedVerificationToken()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        await sut.Handle(CreateNotification(),CancellationToken.None);

        // Assert
        _tokenHasherMock.Verify(x => x.Hash(VerificationToken),Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldAddVerificationToken()
    {
        // Arrange
        EmailVerificationToken? savedToken = null;
        _emailVerificationTokenRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<EmailVerificationToken>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailVerificationToken, CancellationToken>(
                (token, _) => savedToken = token)
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        var entity = Assert.IsType<EmailVerificationToken>(
            savedToken);

        Assert.Equal(
            _userId,
            entity.UserId);

        Assert.Equal(
            TokenHash,
            entity.TokenHash);

        Assert.True(
            entity.ExpiresAt > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Handle_ShouldSaveChanges()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPublishEmailVerificationRequestedEvent()
    {
        // Arrange
        EmailVerificationRequestedEvent? publishedEvent = null;

        _mediatorMock
            .Setup(x => x.Publish(
                It.IsAny<EmailVerificationRequestedEvent>(),
                It.IsAny<CancellationToken>()))
            .Callback<
                INotification,
                CancellationToken>(
                (notification, _) =>
                {
                    publishedEvent =
                        Assert.IsType<EmailVerificationRequestedEvent>(
                            notification);
                })
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        var @event = Assert.IsType<EmailVerificationRequestedEvent>(
            publishedEvent);

        Assert.Equal(
            _userId,
            @event.UserId);

        Assert.Equal(
            UserName,
            @event.UserName);

        Assert.Equal(
            Email,
            @event.Email);

        Assert.Equal(
            VerificationToken,
            @event.VerificationToken);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToDependencies()
    {
        // Arrange
        using var cancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            cancellationTokenSource.Token;

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            cancellationToken);

        // Assert
        _emailVerificationTokenRepositoryMock.Verify(
            x => x.DeleteByUserIdAsync(
                _userId,
                cancellationToken),
            Times.Once);

        _emailVerificationTokenRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<EmailVerificationToken>(),
                cancellationToken),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                cancellationToken),
            Times.Once);

        _mediatorMock.Verify(
            x => x.Publish(
                It.IsAny<EmailVerificationRequestedEvent>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldExecuteOperationsInExpectedOrder()
    {
        // Arrange
        var sequence = new MockSequence();

        _emailVerificationTokenRepositoryMock
            .InSequence(sequence)
            .Setup(x => x.DeleteByUserIdAsync(
                _userId,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _tokenGeneratorMock
            .InSequence(sequence)
            .Setup(x => x.GenerateEmailVerificationToken())
            .Returns(VerificationToken);

        _tokenHasherMock
            .InSequence(sequence)
            .Setup(x => x.Hash(VerificationToken))
            .Returns(TokenHash);

        _emailVerificationTokenRepositoryMock
            .InSequence(sequence)
            .Setup(x => x.AddAsync(
                It.IsAny<EmailVerificationToken>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.InSequence(sequence)
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mediatorMock
            .InSequence(sequence)
            .Setup(x => x.Publish(
                It.IsAny<EmailVerificationRequestedEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        // Assert
        _emailVerificationTokenRepositoryMock.Verify(
            x => x.DeleteByUserIdAsync(
                _userId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateTokenWithThirtyMinuteExpiry()
    {
        // Arrange
        EmailVerificationToken? savedToken = null;

        _emailVerificationTokenRepositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<EmailVerificationToken>(),
                It.IsAny<CancellationToken>()))
            .Callback<EmailVerificationToken, CancellationToken>(
                (token, _) => savedToken = token)
            .Returns(Task.CompletedTask);

        var before = DateTimeOffset.UtcNow;

        var sut = CreateSut();

        // Act
        await sut.Handle(
            CreateNotification(),
            CancellationToken.None);

        var after = DateTimeOffset.UtcNow;

        // Assert
        var entity = Assert.IsType<EmailVerificationToken>(
            savedToken);

        var minimumExpiry =
            before.AddMinutes(30);

        var maximumExpiry =
            after.AddMinutes(30);

        Assert.InRange(
            entity.ExpiresAt,
            minimumExpiry,
            maximumExpiry);
    }

    private UserRegisteredEventHandler CreateSut()
    {
        return new UserRegisteredEventHandler(
            _emailVerificationTokenRepositoryMock.Object,
            _tokenGeneratorMock.Object,
            _tokenHasherMock.Object,
            _unitOfWorkMock.Object,
            _mediatorMock.Object);
    }

    private UserRegisteredEvent CreateNotification()
    {
        return new UserRegisteredEvent(
            _userId,
            UserName,
            Email);
    }
}