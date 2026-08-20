namespace BookMyHall.Application.Common.Interfaces.Storage;

public interface IR2StorageService
{
    Task UploadAsync(
        Stream stream,
        string objectKey,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string objectKey,
        CancellationToken cancellationToken = default);

    Task<Stream?> GetAsync(
        string objectKey,
        CancellationToken cancellationToken = default);

    Task<string?> GetPreSignedUrlAsync(
        string objectKey,
        TimeSpan expiration,
        CancellationToken cancellationToken = default);
}