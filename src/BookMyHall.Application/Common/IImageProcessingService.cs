namespace BookMyHall.Application.Common.Interfaces.Storage;

public interface IImageProcessingService
{
    Task<Stream> CreateThumbnailAsync(
        Stream originalStream,
        int width,
        int height,
        int quality,
        CancellationToken cancellationToken = default);
}