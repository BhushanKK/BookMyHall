using BookMyHall.Application.Common.Interfaces.Storage;
using SkiaSharp;

namespace BookMyHall.Infrastructure.Storage.CloudflareR2;

public sealed class ImageProcessingService : IImageProcessingService
{
    public async Task<Stream> CreateThumbnailAsync(
        Stream originalStream,
        int width,
        int height,
        int quality,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(originalStream);

        if (originalStream.CanSeek)
        {
            originalStream.Position = 0;
        }

        using var inputMemoryStream = new MemoryStream();

        await originalStream.CopyToAsync(
            inputMemoryStream,
            cancellationToken);

        inputMemoryStream.Position = 0;

        using var originalBitmap = SKBitmap.Decode(
            inputMemoryStream);

        if (originalBitmap is null)
        {
            throw new InvalidOperationException(
                "Unable to decode the image.");
        }

        var sourceWidth = originalBitmap.Width;
        var sourceHeight = originalBitmap.Height;

        if (sourceWidth <= 0 || sourceHeight <= 0)
        {
            throw new InvalidOperationException(
                "Invalid image dimensions.");
        }

        var scale = Math.Min(
            (double)width / sourceWidth,
            (double)height / sourceHeight);

        var targetWidth = Math.Max(
            1,
            (int)Math.Round(sourceWidth * scale));

        var targetHeight = Math.Max(
            1,
            (int)Math.Round(sourceHeight * scale));

        using var resizedBitmap = originalBitmap.Resize(
            new SKImageInfo(
                targetWidth,
                targetHeight),
            new SKSamplingOptions(
                SKFilterMode.Linear));

        if (resizedBitmap is null)
        {
            throw new InvalidOperationException(
                "Unable to resize the image.");
        }

        using var image = SKImage.FromBitmap(
            resizedBitmap);

        using var encodedData = image.Encode(
            SKEncodedImageFormat.Webp,
            Math.Clamp(quality, 1, 100));

        if (encodedData is null)
        {
            throw new InvalidOperationException(
                "Unable to encode the thumbnail as WebP.");
        }

        var thumbnailStream = new MemoryStream();

        encodedData.SaveTo(thumbnailStream);

        thumbnailStream.Position = 0;

        return thumbnailStream;
    }
}