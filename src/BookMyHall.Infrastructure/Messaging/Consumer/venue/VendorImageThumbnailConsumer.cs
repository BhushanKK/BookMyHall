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

public sealed class VendorImageThumbnailConsumer(
    IOptions<RabbitMqOptions> rabbitMqOptions,
    IOptions<ImageProcessingOptions> imageProcessingOptions,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<VendorImageThumbnailConsumer> logger)
    : BackgroundService
{
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;
    private readonly ImageProcessingOptions _imageProcessingOptions = imageProcessingOptions.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.HostName,
                Port = _rabbitMqOptions.Port,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password,
                VirtualHost = _rabbitMqOptions.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
            await _channel.ExchangeDeclareAsync(
                _rabbitMqOptions.ExchangeName,
                ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);
            await _channel.QueueDeclareAsync(
                RabbitMqKeys.VendorImageUploadedQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);
            await _channel.QueueBindAsync(
                RabbitMqKeys.VendorImageUploadedQueueName,
                _rabbitMqOptions.ExchangeName,
                RabbitMqKeys.VendorImageUploadedRoutingKey,
                cancellationToken: stoppingToken);
            await _channel.BasicQosAsync(0, 1, false, stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += (_, eventArgs) => ProcessMessageAsync(eventArgs, stoppingToken);
            await _channel.BasicConsumeAsync(
                RabbitMqKeys.VendorImageUploadedQueueName,
                autoAck: false,
                consumer,
                stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Vendor image thumbnail consumer terminated unexpectedly.");
            throw;
        }
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken stoppingToken)
    {
        if (_channel is null)
        {
            return;
        }

        try
        {
            var json = Encoding.UTF8.GetString(eventArgs.Body.Span);
            var message = JsonSerializer.Deserialize<VendorImageUploadedMessage>(json)
                ?? throw new InvalidOperationException("Unable to deserialize VendorImageUploadedMessage.");

            using var scope = serviceScopeFactory.CreateScope();
            var storage = scope.ServiceProvider.GetRequiredService<IR2StorageService>();
            var imageProcessor = scope.ServiceProvider.GetRequiredService<IImageProcessingService>();
            var imageRepository = scope.ServiceProvider.GetRequiredService<IVendorImageRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            var image = await imageRepository.GetByIdAsync(message.VendorImageId, stoppingToken);
            if (image is null || !image.IsActive || image.ImageUrl != message.ObjectKey)
            {
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, stoppingToken);
                return;
            }

            if (!await storage.ExistsAsync(message.ObjectKey, stoppingToken))
            {
                throw new FileNotFoundException("Original vendor image was not found in R2.");
            }

            var originalStream = await storage.GetAsync(message.ObjectKey, stoppingToken)
                ?? throw new FileNotFoundException("Original vendor image could not be downloaded from R2.");

            string thumbnailKey;
            await using (originalStream)
            {
                await using var thumbnailStream = await imageProcessor.CreateThumbnailAsync(
                    originalStream,
                    _imageProcessingOptions.ThumbnailWidth,
                    _imageProcessingOptions.ThumbnailHeight,
                    _imageProcessingOptions.ThumbnailQuality,
                    stoppingToken);

                thumbnailKey = $"vendors/{message.VendorId}/thumbnails/{message.VendorImageId}.webp";
                await storage.UploadAsync(thumbnailStream, thumbnailKey, "image/webp", stoppingToken);
            }

            image.SetThumbnailUrl(thumbnailKey);
            await imageRepository.UpdateAsync(image, stoppingToken);
            await unitOfWork.SaveChangesAsync(stoppingToken);
            await cacheService.RemoveByPrefixAsync(CacheKeys.VendorImagesPaged, stoppingToken);
            await cacheService.RemoveAsync(
                $"{CacheKeys.VendorImage}:{message.VendorImageId}",
                stoppingToken);
            await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to process vendor image thumbnail.");
            try
            {
                await _channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception nackException)
            {
                logger.LogError(nackException, "Failed to NACK vendor image thumbnail message.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}