namespace BookMyHall.Infrastructure.Email;

public static class ImageService
{
    public static string ToBase64(string imagePath)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException($"Image not found: {imagePath}");

        var bytes = File.ReadAllBytes(imagePath);
        return Convert.ToBase64String(bytes);
    }
}