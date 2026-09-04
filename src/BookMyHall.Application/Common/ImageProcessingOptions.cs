namespace BookMyHall.Application.Common.Options;

public sealed class ImageProcessingOptions
{
    public const string SectionName = "ImageProcessing";
    public int ThumbnailWidth { get; set; } = 400;
    public int ThumbnailHeight { get; set; } = 300;
    public int ThumbnailQuality { get; set; } = 80;
}