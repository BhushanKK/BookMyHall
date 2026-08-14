namespace BookMyHall.Shared.Common;

public static class FileContents
{
    public static string GetContentType(string objectKey)
    {
        var extension = Path.GetExtension(objectKey);

        return extension.ToLowerInvariant() switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            ".avif" => "image/avif",
            _ => "application/octet-stream"
        };
    }
}