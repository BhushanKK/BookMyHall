using Amazon.S3;
using Amazon.S3.Model;

using BookMyHall.Application.Common.Interfaces.Storage;

using Microsoft.Extensions.Options;

namespace BookMyHall.Infrastructure.Storage.CloudflareR2;

public sealed class CloudflareR2StorageService(
    IAmazonS3 s3Client,
    IOptions<CloudflareR2Options> options)
    : IR2StorageService
{
    private readonly CloudflareR2Options _options =
        options.Value;


    /* =========================================================
       UPLOAD
    ========================================================= */

    public async Task UploadAsync(
        Stream stream,
        string objectKey,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(
                nameof(stream));
        }

        if (string.IsNullOrWhiteSpace(objectKey))
        {
            throw new ArgumentException(
                "Object key is required.",
                nameof(objectKey));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException(
                "Content type is required.",
                nameof(contentType));
        }

        /*
         * Make sure the stream starts from the beginning.
         *
         * This is especially important when the stream has
         * previously been read by another operation.
         */

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey,
            InputStream = stream,
            ContentType = contentType,

            /*
             * R2 works better with chunked encoding disabled.
             */
            UseChunkEncoding = false
        };

        await s3Client.PutObjectAsync(
            request,
            cancellationToken);
    }


    /* =========================================================
       DELETE
    ========================================================= */

    public async Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            throw new ArgumentException(
                "Object key is required.",
                nameof(objectKey));
        }

        var request = new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectKey
        };

        await s3Client.DeleteObjectAsync(
            request,
            cancellationToken);
    }


    /* =========================================================
       EXISTS
    ========================================================= */

    public async Task<bool> ExistsAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return false;
        }

        try
        {
            var request =
                new GetObjectMetadataRequest
                {
                    BucketName =
                        _options.BucketName,

                    Key =
                        objectKey
                };

            await s3Client.GetObjectMetadataAsync(
                request,
                cancellationToken);

            return true;
        }
        catch (AmazonS3Exception ex)
            when (ex.StatusCode ==
                  System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }


    /* =========================================================
       DOWNLOAD
    ========================================================= */

    public async Task<Stream?> GetAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return null;
        }

        try
        {
            var request =
                new GetObjectRequest
                {
                    BucketName =
                        _options.BucketName,

                    Key =
                        objectKey
                };

            var response =
                await s3Client.GetObjectAsync(
                    request,
                    cancellationToken);

            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex)
            when (ex.StatusCode ==
                  System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }


    /* =========================================================
       PRE-SIGNED URL
    =========================================================
    
       IMPORTANT:
       
       We intentionally DO NOT call ExistsAsync() here.

       Previous implementation:

           ExistsAsync()
                ↓
           GetPreSignedURL()

       That creates an unnecessary R2 request for every image.

       The database already contains the object key.
       R2 will validate the object when the generated URL
       is actually requested.
    
    ========================================================= */

    public Task<string?> GetPreSignedUrlAsync(
        string objectKey,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return Task.FromResult<string?>(null);
        }

        if (expiration <= TimeSpan.Zero)
        {
            throw new ArgumentException(
                "Expiration must be greater than zero.",
                nameof(expiration));
        }

        /*
         * R2 object keys should not contain leading "/".
         *
         * Example:
         *
         * Correct:
         * halls/abc/image.jpg
         *
         * Avoid:
         * /halls/abc/image.jpg
         */

        var normalizedObjectKey =
            objectKey.TrimStart('/');

        /*
         * Generate an HTTPS GET pre-signed URL.
         */

        var request =
            new GetPreSignedUrlRequest
            {
                BucketName =
                    _options.BucketName,

                Key =
                    normalizedObjectKey,

                Verb =
                    HttpVerb.GET,

                Protocol =
                    Protocol.HTTPS,

                Expires =
                    DateTime.UtcNow.Add(expiration)
            };

        var url =
            s3Client.GetPreSignedURL(request);

        return Task.FromResult<string?>(url);
    }
}