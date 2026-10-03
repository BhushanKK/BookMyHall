using AutoMapper;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Abstractions.Security;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Domain.Constants;
using BookMyHall.Domain.Venue;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BookMyHall.Application.Tests.Features.Venue.VendorLogo;

public sealed class VendorLogoTests
{
    private readonly Vendor _vendor = new() { VendorId = Guid.NewGuid(), UserId = Guid.NewGuid(), LogoUrl = "old-logo.png" };
    private readonly Mock<IVendorRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IR2StorageService> _storage = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<ICurrentUser> _user = new();

    public VendorLogoTests()
    {
        _repository.Setup(x => x.GetByIdAsync(_vendor.VendorId, It.IsAny<CancellationToken>())).ReturnsAsync(_vendor);
        _user.SetupGet(x => x.Roles).Returns([RoleConstants.Vendor]);
        _user.SetupGet(x => x.UserId).Returns(_vendor.UserId);
        _storage.Setup(x => x.GetPreSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://example.test/signed-logo.png");
    }

    [Fact]
    public async Task Upload_ReplacesLogoAfterSavingAndInvalidatesVendorCaches()
    {
        var oldKey = _vendor.LogoUrl;
        var calls = new List<string>();
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => calls.Add("save")).ReturnsAsync(1);
        _storage.Setup(x => x.DeleteAsync(oldKey!, It.IsAny<CancellationToken>()))
            .Callback(() => calls.Add("delete")).Returns(Task.CompletedTask);

        using var stream = new MemoryStream([1, 2, 3]);
        var response = await UploadHandler().Handle(Command(stream), CancellationToken.None);

        response.StatusCode.Should().Be(200);
        response.Data!.LogoUrl.Should().Be("https://example.test/signed-logo.png");
        _vendor.LogoUrl.Should().StartWith($"vendors/{_vendor.VendorId}/logos/").And.EndWith(".png");
        calls.Should().Equal("save", "delete");
        _cache.Verify(x => x.RemoveAsync($"{CacheKeys.Vendors}:{_vendor.VendorId}", It.IsAny<CancellationToken>()), Times.Once);
        _cache.Verify(x => x.RemoveByPrefixAsync($"{CacheKeys.VendorsPaged}:", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(0, "logo.png", "image/png")]
    [InlineData(5242881, "logo.png", "image/png")]
    [InlineData(3, "logo.svg", "image/svg+xml")]
    [InlineData(3, "logo.png", "image/jpeg")]
    public async Task Upload_RejectsInvalidFilesBeforeStorage(long size, string name, string contentType)
    {
        using var stream = new MemoryStream([1, 2, 3]);
        var response = await UploadHandler().Handle(
            new UploadVendorLogoCommand(_vendor.VendorId, stream, name, contentType, size), CancellationToken.None);

        response.StatusCode.Should().Be(400);
        _storage.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Upload_RejectsAnotherVendorsLogo()
    {
        _user.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        using var stream = new MemoryStream([1, 2, 3]);

        var response = await UploadHandler().Handle(Command(stream), CancellationToken.None);

        response.StatusCode.Should().Be(403);
        _storage.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Upload_ReturnsNotFoundForMissingVendor()
    {
        _repository.Setup(x => x.GetByIdAsync(_vendor.VendorId, It.IsAny<CancellationToken>())).ReturnsAsync((Vendor?)null);
        using var stream = new MemoryStream([1, 2, 3]);

        var response = await UploadHandler().Handle(Command(stream), CancellationToken.None);

        response.StatusCode.Should().Be(404);
        _storage.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Upload_OnSaveFailureDeletesNewObjectAndPreservesOldLogo()
    {
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Save failed"));
        using var stream = new MemoryStream([1, 2, 3]);

        var action = () => UploadHandler().Handle(Command(stream), CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>();
        _vendor.LogoUrl.Should().Be("old-logo.png");
        _storage.Verify(x => x.DeleteAsync(It.Is<string>(key => key.StartsWith($"vendors/{_vendor.VendorId}/logos/")), CancellationToken.None), Times.Once);
        _storage.Verify(x => x.DeleteAsync("old-logo.png", It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ClearsLogoAndRemovesStorageObject()
    {
        var handler = new DeleteVendorLogoCommandHandler(_repository.Object, _unitOfWork.Object,
            _storage.Object, _cache.Object, _user.Object, NullLogger<DeleteVendorLogoCommandHandler>.Instance);

        var response = await handler.Handle(new DeleteVendorLogoCommand(_vendor.VendorId), CancellationToken.None);

        response.StatusCode.Should().Be(200);
        _vendor.LogoUrl.Should().BeNull();
        _storage.Verify(x => x.DeleteAsync("old-logo.png", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Delete_RejectsAnotherVendor()
    {
        _user.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        var handler = new DeleteVendorLogoCommandHandler(_repository.Object, _unitOfWork.Object,
            _storage.Object, _cache.Object, _user.Object, NullLogger<DeleteVendorLogoCommandHandler>.Instance);

        var response = await handler.Handle(new DeleteVendorLogoCommand(_vendor.VendorId), CancellationToken.None);

        response.StatusCode.Should().Be(403);
        _vendor.LogoUrl.Should().Be("old-logo.png");
        _unitOfWork.VerifyNoOtherCalls();
        _storage.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Get_ReturnsSignedUrl()
    {
        var response = await new GetVendorLogoQueryHandler(_repository.Object, _storage.Object)
            .Handle(new GetVendorLogoQuery(_vendor.VendorId), CancellationToken.None);

        response.StatusCode.Should().Be(200);
        response.Data!.LogoUrl.Should().Be("https://example.test/signed-logo.png");
    }

    [Fact]
    public async Task Get_WithoutLogoReturnsNotFound()
    {
        _vendor.LogoUrl = null;
        var response = await new GetVendorLogoQueryHandler(_repository.Object, _storage.Object)
            .Handle(new GetVendorLogoQuery(_vendor.VendorId), CancellationToken.None);

        response.StatusCode.Should().Be(404);
        _storage.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetContent_ReturnsStoredStreamAndMimeType()
    {
        using var stream = new MemoryStream([1, 2, 3]);
        _storage.Setup(x => x.GetAsync("old-logo.png", It.IsAny<CancellationToken>())).ReturnsAsync(stream);

        var result = await new GetVendorLogoContentQueryHandler(_repository.Object, _storage.Object)
            .Handle(new GetVendorLogoContentQuery(_vendor.VendorId), CancellationToken.None);

        result!.Stream.Should().BeSameAs(stream);
        result.ContentType.Should().Be("image/png");
    }

    [Fact]
    public void VendorUpdateMapping_PreservesStoredLogoAndReturnsContentUrl()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile<VendorMappingProfile>(), NullLoggerFactory.Instance).CreateMapper();

        mapper.Map(new UpdateVendorCommand { BusinessName = "Updated business", LogoUrl = "untrusted-key" }, _vendor);

        _vendor.LogoUrl.Should().Be("old-logo.png");
        mapper.Map<VendorDto>(_vendor).LogoUrl.Should().Be($"/api/vendors/{_vendor.VendorId}/logo/content");
    }

    private UploadVendorLogoCommand Command(Stream stream)
        => new(_vendor.VendorId, stream, "logo.png", "image/png", 3);

    private UploadVendorLogoCommandHandler UploadHandler()
        => new(_repository.Object, _unitOfWork.Object, _storage.Object, _cache.Object,
            _user.Object, NullLogger<UploadVendorLogoCommandHandler>.Instance);
}
