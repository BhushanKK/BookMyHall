using BookMyHall.Infrastructure.Email;

namespace BookMyHall.Infrastructure.Tests.Email;

public sealed class ImageServiceTests : IDisposable
{
    private readonly string _tempFolderPath;

    public ImageServiceTests()
    {
        _tempFolderPath = Path.Combine(Path.GetTempPath(), "ImageServiceTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempFolderPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempFolderPath))
        {
            Directory.Delete(_tempFolderPath, recursive: true);
        }
    }

    [Fact]
    public void ToBase64_WhenFileExists_ShouldReturnCorrectBase64String()
    {
        // Arrange
        var filePath = Path.Combine(_tempFolderPath, "sample-image.png");
        byte[] expectedBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        File.WriteAllBytes(filePath, expectedBytes);

        var expectedBase64 = Convert.ToBase64String(expectedBytes);

        // Act
        var result = ImageService.ToBase64(filePath);

        // Assert
        Assert.Equal(expectedBase64, result);
    }

    [Fact]
    public void ToBase64_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_tempFolderPath, "nonexistent.png");

        // Act & Assert
        var exception = Assert.Throws<FileNotFoundException>(() => ImageService.ToBase64(nonExistentPath));
        Assert.Contains($"Image not found: {nonExistentPath}", exception.Message);
    }

    [Fact]
    public void ToBase64_WhenFileIsEmpty_ShouldReturnEmptyString()
    {
        // Arrange
        var filePath = Path.Combine(_tempFolderPath, "empty.png");
        File.WriteAllBytes(filePath, Array.Empty<byte>());

        // Act
        var result = ImageService.ToBase64(filePath);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}