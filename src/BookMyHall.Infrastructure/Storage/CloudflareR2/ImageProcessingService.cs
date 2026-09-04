using BookMyHall.Application.Common.Interfaces.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

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

        using var image = await Image.LoadAsync(
            originalStream,
            cancellationToken);

        image.Mutate(x =>
            x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Max
            }));

        var thumbnailStream = new MemoryStream();

        var encoder = new WebpEncoder
        {
            Quality = quality
        };

        await image.SaveAsWebpAsync(
            thumbnailStream,
            encoder,
            cancellationToken);

        thumbnailStream.Position = 0;

        return thumbnailStream;
    }
}