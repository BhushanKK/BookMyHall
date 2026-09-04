using System.Text;
using System.Text.Json;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Application.Common.Options;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BookMyHall.Infrastructure.Messaging.Consumers;

public sealed class HallImageThumbnailConsumer(
    IOptions<RabbitMqOptions> rabbitMqOptions,
    IOptions<ImageProcessingOptions> imageProcessingOptions,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<HallImageThumbnailConsumer> logger)
    : BackgroundService
{
    private readonly RabbitMqOptions _rabbitMqOptions =
        rabbitMqOptions.Value;

    private readonly ImageProcessingOptions _imageProcessingOptions =
        imageProcessingOptions.Value;

    private IConnection? _connection;
    private IChannel? _channel;

    private const string QueueName =
        RabbitMqKeys.HallImageUploadedQueueName;

    private const string RoutingKey =
        RabbitMqKeys.HallImageUploadedRoutingKey;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation(
                "HallImageThumbnailConsumer starting.");

            // -----------------------------------------------------
            // 1. Create RabbitMQ connection
            // -----------------------------------------------------
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost
            };

            _connection =
                await factory.CreateConnectionAsync(
                    stoppingToken);

            logger.LogInformation(
                "HallImageThumbnailConsumer RabbitMQ connection created.");

            // -----------------------------------------------------
            // 2. Create RabbitMQ channel
            // -----------------------------------------------------
            _channel =
                await _connection.CreateChannelAsync(
                    cancellationToken: stoppingToken);

            logger.LogInformation(
                "HallImageThumbnailConsumer RabbitMQ channel created.");

            // -----------------------------------------------------
            // 3. Declare exchange
            // -----------------------------------------------------
            await _channel.ExchangeDeclareAsync(
                exchange: _rabbitMqOptions.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            // -----------------------------------------------------
            // 4. Declare queue
            // -----------------------------------------------------
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            // -----------------------------------------------------
            // 5. Bind queue
            // -----------------------------------------------------
            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: _rabbitMqOptions.ExchangeName,
                routingKey: RoutingKey,
                cancellationToken: stoppingToken);

            // -----------------------------------------------------
            // 6. Process one message at a time
            // -----------------------------------------------------
            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: stoppingToken);

            // -----------------------------------------------------
            // 7. Create consumer
            // -----------------------------------------------------
            var consumer =
                new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync +=
                async (_, eventArgs) =>
                {
                    await ProcessMessageAsync(
                        eventArgs,
                        stoppingToken);
                };

            // -----------------------------------------------------
            // 8. Start consuming
            // -----------------------------------------------------
            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "HallImageThumbnailConsumer started successfully. " +
                "Queue: {QueueName}, RoutingKey: {RoutingKey}",
                QueueName,
                RoutingKey);

            // -----------------------------------------------------
            // Keep BackgroundService alive
            // -----------------------------------------------------
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "HallImageThumbnailConsumer stopping.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "HallImageThumbnailConsumer terminated unexpectedly.");

            throw;
        }
    }

    private async Task ProcessMessageAsync(
        BasicDeliverEventArgs eventArgs,
        CancellationToken stoppingToken)
    {
        if (_channel is null)
        {
            logger.LogError(
                "RabbitMQ channel is not available.");

            return;
        }

        try
        {
            // -----------------------------------------------------
            // 1. Deserialize message
            // -----------------------------------------------------
            var json =
                Encoding.UTF8.GetString(
                    eventArgs.Body.Span);

            var message =
                JsonSerializer.Deserialize<HallImageUploadedMessage>(
                    json);

            if (message is null)
            {
                throw new InvalidOperationException(
                    "Unable to deserialize HallImageUploadedMessage.");
            }

            logger.LogInformation(
                "Processing Hall image thumbnail. " +
                "HallImageId: {HallImageId}, " +
                "HallId: {HallId}, " +
                "ObjectKey: {ObjectKey}",
                message.HallImageId,
                message.HallId,
                message.ObjectKey);

            // -----------------------------------------------------
            // 2. Create application scope
            // -----------------------------------------------------
            using var scope =
                serviceScopeFactory.CreateScope();

            var r2StorageService =
                scope.ServiceProvider
                    .GetRequiredService<IR2StorageService>();

            var imageProcessingService =
                scope.ServiceProvider
                    .GetRequiredService<IImageProcessingService>();

            var hallImageRepository =
                scope.ServiceProvider
                    .GetRequiredService<IHallImageRepository>();

            var unitOfWork =
                scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

            var cacheService =
                scope.ServiceProvider
                    .GetRequiredService<ICacheService>();

            // -----------------------------------------------------
            // 3. Verify original image exists in R2
            // -----------------------------------------------------
            var originalExists =
                await r2StorageService.ExistsAsync(
                    message.ObjectKey,
                    stoppingToken);

            if (!originalExists)
            {
                throw new FileNotFoundException(
                    "Original Hall image was not found in R2. " +
                    $"ObjectKey: {message.ObjectKey}");
            }

            logger.LogInformation(
                "Original Hall image exists in R2. " +
                "ObjectKey: {ObjectKey}",
                message.ObjectKey);

            // -----------------------------------------------------
            // 4. Download original image from R2
            // -----------------------------------------------------
            var originalStream =
                await r2StorageService.GetAsync(
                    message.ObjectKey,
                    stoppingToken);

            if (originalStream is null)
            {
                throw new FileNotFoundException(
                    "Original Hall image could not be downloaded " +
                    "from R2. " +
                    $"ObjectKey: {message.ObjectKey}");
            }

            await using (originalStream)
            {
                // -------------------------------------------------
                // 5. Generate thumbnail
                // -------------------------------------------------
                await using var thumbnailStream =
                    await imageProcessingService.CreateThumbnailAsync(
                        originalStream,
                        _imageProcessingOptions.ThumbnailWidth,
                        _imageProcessingOptions.ThumbnailHeight,
                        _imageProcessingOptions.ThumbnailQuality,
                        stoppingToken);

                if (thumbnailStream is null)
                {
                    throw new InvalidOperationException(
                        "Thumbnail generation returned a null stream.");
                }

                // -------------------------------------------------
                // 6. Create thumbnail object key
                // -------------------------------------------------
                var thumbnailObjectKey =
                    $"halls/{message.HallId}/thumbnails/" +
                    $"{message.HallImageId}.webp";

                // -------------------------------------------------
                // 7. Upload thumbnail to R2
                // -------------------------------------------------
                await r2StorageService.UploadAsync(
                    thumbnailStream,
                    thumbnailObjectKey,
                    "image/webp",
                    stoppingToken);

                logger.LogInformation(
                    "Hall image thumbnail uploaded successfully. " +
                    "HallImageId: {HallImageId}, " +
                    "ThumbnailObjectKey: {ThumbnailObjectKey}",
                    message.HallImageId,
                    thumbnailObjectKey);

                // -------------------------------------------------
                // 8. Get HallImage from database
                // -------------------------------------------------
                var hallImage =
                    await hallImageRepository.GetByIdAsync(
                        message.HallImageId,
                        stoppingToken);

                if (hallImage is null)
                {
                    throw new InvalidOperationException(
                        $"HallImage '{message.HallImageId}' was not found.");
                }

                // -------------------------------------------------
                // 9. Update ThumbnailUrl
                // -------------------------------------------------
                hallImage.SetThumbnailUrl(
                    thumbnailObjectKey);

                // -------------------------------------------------
                // 10. Update repository
                // -------------------------------------------------
                await hallImageRepository.UpdateAsync(
                    hallImage,
                    stoppingToken);

                // -------------------------------------------------
                // 11. Save database
                // -------------------------------------------------
                await unitOfWork.SaveChangesAsync(
                    stoppingToken);

                logger.LogInformation(
                    "HallImage ThumbnailUrl updated successfully. " +
                    "HallImageId: {HallImageId}",
                    message.HallImageId);

                // -------------------------------------------------
                // 12. Clear Hall image cache
                // -------------------------------------------------
                await cacheService.RemoveByPrefixAsync(
                    $"{CacheKeys.HallImagesPaged}:",
                    stoppingToken);
            }

            // -----------------------------------------------------
            // 13. ACK RabbitMQ message
            // -----------------------------------------------------
            await _channel.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "Hall image thumbnail processing completed successfully. " +
                "HallImageId: {HallImageId}",
                message.HallImageId);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "Hall image thumbnail processing cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error processing Hall image thumbnail. " +
                "DeliveryTag: {DeliveryTag}",
                eventArgs.DeliveryTag);

            // -----------------------------------------------------
            // Do not requeue currently.
            //
            // Failed messages will be removed from the queue.
            // We can add retry/DLQ later.
            // -----------------------------------------------------
            try
            {
                await _channel.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception nackException)
            {
                logger.LogError(
                    nackException,
                    "Failed to NACK Hall image thumbnail message. " +
                    "DeliveryTag: {DeliveryTag}",
                    eventArgs.DeliveryTag);
            }
        }
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Stopping HallImageThumbnailConsumer.");

        try
        {
            if (_channel is not null)
            {
                await _channel.CloseAsync(
                    cancellationToken);
            }

            if (_connection is not null)
            {
                await _connection.CloseAsync(
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Error while closing HallImageThumbnailConsumer " +
                "RabbitMQ resources.");
        }

        await base.StopAsync(
            cancellationToken);
    }
}