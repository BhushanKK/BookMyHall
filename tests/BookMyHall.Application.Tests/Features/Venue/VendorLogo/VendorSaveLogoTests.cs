using AutoMapper;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BookMyHall.Application.Tests.Features.Venue.VendorLogo;

public sealed class VendorSaveLogoTests
{
    private readonly Mock<IVendorRepository> _vendors = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IR2StorageService> _storage = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IMessageHelper> _messages = new();
    private readonly IMapper _mapper = new MapperConfiguration(config => config.AddProfile<VendorMappingProfile>(), NullLoggerFactory.Instance).CreateMapper();
    private readonly Vendor _vendor = new() { VendorId = Guid.NewGuid(), LogoUrl = "old-logo.png" };

    public VendorSaveLogoTests()
    {
        _vendors.Setup(x => x.GetByIdAsync(_vendor.VendorId, It.IsAny<CancellationToken>())).ReturnsAsync(_vendor);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Save_UploadsLogoWithVendorAndReturnsContentUrl(bool create)
    {
        using var stream = new MemoryStream([1, 2, 3]);
        var logo = new VendorLogoUpload(stream, "logo.png", "image/png", 3);

        var result = await SaveAsync(create, logo);

        result.Success.Should().BeTrue();
        result.Data!.LogoUrl.Should().EndWith("/logo/content");
        _storage.Verify(x => x.UploadAsync(stream, It.Is<string>(key => key.Contains("/logos/") && key.EndsWith(".png")), "image/png", It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        if (!create)
        {
            _vendor.LogoUrl.Should().NotBe("old-logo.png");
            _storage.Verify(x => x.DeleteAsync("old-logo.png", CancellationToken.None), Times.Once);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Save_OnDatabaseFailureCleansNewUpload(bool create)
    {
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Save failed"));
        using var stream = new MemoryStream([1, 2, 3]);
        var action = () => SaveAsync(create, new VendorLogoUpload(stream, "logo.png", "image/png", 3));

        await action.Should().ThrowAsync<InvalidOperationException>();

        _storage.Verify(x => x.DeleteAsync(It.Is<string>(key => key.Contains("/logos/")), CancellationToken.None), Times.Once);
        _storage.Verify(x => x.DeleteAsync("old-logo.png", It.IsAny<CancellationToken>()), Times.Never);
        _vendor.LogoUrl.Should().Be("old-logo.png");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Save_RejectsInvalidLogoBeforePersistence(bool create)
    {
        using var stream = new MemoryStream([1, 2, 3]);

        var result = await SaveAsync(create, new VendorLogoUpload(stream, "logo.exe", "image/png", 3));

        result.StatusCode.Should().Be(400);
        _storage.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_WithoutFilePreservesLogo()
    {
        var result = await SaveAsync(false, null);

        result.Success.Should().BeTrue();
        _vendor.LogoUrl.Should().Be("old-logo.png");
        _storage.VerifyNoOtherCalls();
    }

    private Task<BookMyHall.Contracts.Common.ApiResponse<VendorDto>> SaveAsync(bool create, VendorLogoUpload? logo)
    {
        if (create)
        {
            var validator = new Mock<IValidator<CreateVendorCommand>>();
            validator.Setup(x => x.ValidateAsync(It.IsAny<CreateVendorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            var handler = new CreateVendorCommandHandler(_vendors.Object, _unitOfWork.Object, _mapper, validator.Object,
                _messages.Object, _cache.Object, _storage.Object, NullLogger<CreateVendorCommandHandler>.Instance);
            return handler.Handle(new CreateVendorCommand { BusinessName = "Vendor", Logo = logo }, CancellationToken.None);
        }

        var updateValidator = new Mock<IValidator<UpdateVendorCommand>>();
        updateValidator.Setup(x => x.ValidateAsync(It.IsAny<UpdateVendorCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        var updateHandler = new UpdateVendorCommandHandler(_vendors.Object, _unitOfWork.Object, _mapper, updateValidator.Object,
            _messages.Object, _cache.Object, _storage.Object, NullLogger<UpdateVendorCommandHandler>.Instance);
        return updateHandler.Handle(new UpdateVendorCommand { VendorId = _vendor.VendorId, BusinessName = "Vendor", Logo = logo }, CancellationToken.None);
    }
}
