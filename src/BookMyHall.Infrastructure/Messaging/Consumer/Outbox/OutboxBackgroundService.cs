using System.Text.Json;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Events.Identity;
using BookMyHall.Domain.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookMyHall.Infrastructure.Messaging;

public sealed class OutboxBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxBackgroundService> logger)
    : BackgroundService
{
    private const int BatchSize = 20;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMessagesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing outbox messages.");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }

        logger.LogInformation("Outbox background service stopped.");
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();

        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var messagePublisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var messages = await outboxRepository.GetPendingAsync(
            BatchSize,
            cancellationToken);

        if (messages.Count == 0)
            return;

        foreach (var message in messages)
        {
            try
            {
                await PublishAsync(message, messagePublisher, cancellationToken);
                message.ProcessedOn = DateTimeOffset.UtcNow;
                message.Error = null;

                logger.LogInformation(
                    "Outbox message processed successfully. OutboxMessageId: {OutboxMessageId}, EventType: {EventType}",
                    message.OutboxMessageId,
                    message.EventType);
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;
                message.UpdatedDate = DateTimeOffset.UtcNow;

                logger.LogError(
                    ex,
                    "Failed to process outbox message. OutboxMessageId: {OutboxMessageId}, EventType: {EventType}, RetryCount: {RetryCount}",
                    message.OutboxMessageId,
                    message.EventType,
                    message.RetryCount);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static async Task PublishAsync(
        OutboxMessage message,
        IMessagePublisher messagePublisher,
        CancellationToken cancellationToken)
    {
        switch (message.EventType)
        {
            case nameof(UserLoggedInEvent):
            {
                var loginEvent = JsonSerializer.Deserialize<UserLoggedInEvent>(
                    message.Payload);

                if (loginEvent is null)
                    throw new InvalidOperationException(
                        $"Unable to deserialize outbox message '{message.OutboxMessageId}' as {nameof(UserLoggedInEvent)}.");

                await messagePublisher.PublishAsync(
                    loginEvent,
                    cancellationToken);

                break;
            }

            default:
                throw new InvalidOperationException(
                    $"Unsupported outbox event type '{message.EventType}'.");
        }
    }
}