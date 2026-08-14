using Amazon.S3;
using Amazon.S3.Model;
using BookMyHall.Application.Common.Interfaces.Storage;
using Microsoft.Extensions.Options;

namespace BookMyHall.Infrastructure.Storage.CloudflareR2;

public sealed class CloudflareR2StorageService(IAmazonS3 s3Client,
    IOptions<CloudflareR2Options> options) : IR2StorageService
{
    public async Task UploadAsync(Stream stream, string objectKey, string contentType,
        CancellationToken cancellationToken = default)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        if (string.IsNullOrWhiteSpace(objectKey))
            throw new ArgumentException("Object key is required.", nameof(objectKey));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type is required.", nameof(contentType));

        var request = new PutObjectRequest
        {
            BucketName = options.Value.BucketName,
            Key = objectKey,
            InputStream = stream,
            ContentType = contentType,
            UseChunkEncoding = false
        };

        await s3Client.PutObjectAsync(request, cancellationToken);
    }

    public async Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
            throw new ArgumentException("Object key is required.", nameof(objectKey));

        var request = new DeleteObjectRequest
        {
            BucketName = options.Value.BucketName,
            Key = objectKey
        };

        await s3Client.DeleteObjectAsync(request, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string objectKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
            return false;

        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = options.Value.BucketName,
                Key = objectKey
            };

            await s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex)
            when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}