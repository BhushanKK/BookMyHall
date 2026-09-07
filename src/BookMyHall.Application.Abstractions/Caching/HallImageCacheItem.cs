namespace BookMyHall.Application.Abstractions.Caching;

/// <summary>
/// Cache representation of a HallImage.
/// 
/// IMPORTANT:
/// This class must contain only persistent image information.
/// 
/// Do NOT store:
/// - R2 pre-signed URLs
/// - temporary URLs
/// - API response objects
/// - EF navigation properties
/// </summary>
public sealed class HallImageCacheItem
{
    public Guid HallImageId { get; init; }

    public Guid HallId { get; init; }

    /// <summary>
    /// Persistent R2 object key for the original image.
    /// Example:
    /// halls/{hallId}/{hallImageId}.jpg
    /// </summary>
    public string ImageUrl { get; init; } = string.Empty;

    /// <summary>
    /// Persistent R2 object key for the thumbnail.
    /// This can be null while RabbitMQ thumbnail generation
    /// is still in progress.
    /// </summary>
    public string? ThumbnailUrl { get; init; }

    public int DisplayOrder { get; init; }

    public bool IsCoverImage { get; init; }

    public bool IsActive { get; init; }

    public bool IsDeleted { get; init; }

    public string? CreatedBy { get; init; }

    public DateTime CreatedDate { get; init; }
}