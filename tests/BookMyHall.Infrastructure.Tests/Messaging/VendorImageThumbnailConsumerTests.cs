using System.Reflection;
using BookMyHall.Application.Common.Options;
using BookMyHall.Infrastructure.Messaging;
using BookMyHall.Infrastructure.Messaging.Consumers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace BookMyHall.Infrastructure.Tests.Messaging;

public sealed class VendorImageThumbnailConsumerTests
{
    [Fact]
    public async Task Stop_WhenChannelAlreadyClosedStillClosesConnection()
    {
        var channel = new Mock<IChannel>();
        channel.SetupGet(x => x.IsOpen).Returns(true);
        channel.Setup(x => x.CloseAsync(It.IsAny<ushort>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AlreadyClosedException(new ShutdownEventArgs(ShutdownInitiator.Peer, 200, "Closed")));
        var connection = new Mock<IConnection>();
        connection.SetupGet(x => x.IsOpen).Returns(true);
        using var consumer = CreateConsumer(channel.Object, connection.Object);

        var action = () => consumer.StopAsync(CancellationToken.None);

        await action.Should().NotThrowAsync();
        channel.Verify(x => x.CloseAsync(It.IsAny<ushort>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        connection.Verify(x => x.CloseAsync(It.IsAny<ushort>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Stop_WithClosedResourcesDoesNotCloseThemAgain()
    {
        var channel = new Mock<IChannel>();
        var connection = new Mock<IConnection>();
        using var consumer = CreateConsumer(channel.Object, connection.Object);

        await consumer.StopAsync(CancellationToken.None);

        channel.Verify(x => x.CloseAsync(It.IsAny<ushort>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        connection.Verify(x => x.CloseAsync(It.IsAny<ushort>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static VendorImageThumbnailConsumer CreateConsumer(IChannel channel, IConnection connection)
    {
        var consumer = new VendorImageThumbnailConsumer(Microsoft.Extensions.Options.Options.Create(new RabbitMqOptions()),
            Microsoft.Extensions.Options.Options.Create(new ImageProcessingOptions()), Mock.Of<IServiceScopeFactory>(), NullLogger<VendorImageThumbnailConsumer>.Instance);
        typeof(VendorImageThumbnailConsumer).GetField("_channel", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(consumer, channel);
        typeof(VendorImageThumbnailConsumer).GetField("_connection", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(consumer, connection);
        return consumer;
    }
}
