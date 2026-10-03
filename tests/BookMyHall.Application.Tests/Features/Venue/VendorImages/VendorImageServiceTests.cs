using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Common.Interfaces.Repositories.Venue;
using BookMyHall.Application.Common.Interfaces.Storage;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using FluentAssertions;
using Moq;

namespace BookMyHall.Application.Tests.Features.Venue.VendorImages;

public sealed class VendorImageServiceTests
{
    private readonly Guid _vendorId = Guid.NewGuid();
    private readonly Guid _serviceId = Guid.NewGuid();
    private readonly Mock<IVendorRepository> _vendors = new();
    private readonly Mock<IVendorServiceRepository> _services = new();
    private readonly Mock<IVendorImageRepository> _images = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IR2StorageService> _storage = new();
    private readonly Mock<IMessagePublisher> _publisher = new();
    private readonly Mock<IMessageHelper> _messages = new();
    private readonly Mock<ICacheService> _cache = new();

    public VendorImageServiceTests()
    {
        _vendors.Setup(x => x.GetByIdAsync(_vendorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Vendor { VendorId = _vendorId });
        SetService(_vendorId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Create_PersistsServiceAssignmentAndScopesCover(bool serviceImage)
    {
        Guid? serviceId = serviceImage ? _serviceId : null;
        using var stream = new MemoryStream([1, 2, 3]);
        var handler = new CreateVendorImageCommandHandler(
            _vendors.Object, _services.Object, _images.Object, _unitOfWork.Object,
            _storage.Object, _publisher.Object, _messages.Object, _cache.Object);

        var response = await handler.Handle(
            new CreateVendorImageCommand(_vendorId, stream, "image.jpg", "image/jpeg", 3, 1, true, serviceId),
            CancellationToken.None);

        response.StatusCode.Should().Be(201);
        _images.Verify(x => x.AddAsync(
            It.Is<VendorImage>(i => i.VendorId == _vendorId && i.VendorServiceId == serviceId),
            It.IsAny<CancellationToken>()), Times.Once);
        _images.Verify(x => x.ClearOtherCoverImagesAsync(
            _vendorId, null, It.IsAny<CancellationToken>(), serviceId), Times.Once);
    }

    [Theory]
    [InlineData("other-vendor", 400)]
    [InlineData("missing", 404)]
    [InlineData("inactive", 404)]
    [InlineData("deleted", 404)]
    [InlineData("empty", 400)]
    public async Task Create_RejectsInvalidServiceBeforeUploading(string state, int expectedStatus)
    {
        SetService(state == "other-vendor" ? Guid.NewGuid() : _vendorId,
            state != "inactive", state == "deleted");
        if (state == "missing")
        {
            _services.Setup(x => x.GetByIdAsync(_serviceId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((VendorService?)null);
        }

        using var stream = new MemoryStream([1, 2, 3]);
        var handler = new CreateVendorImageCommandHandler(
            _vendors.Object, _services.Object, _images.Object, _unitOfWork.Object,
            _storage.Object, _publisher.Object, _messages.Object, _cache.Object);
        var response = await handler.Handle(
            new CreateVendorImageCommand(_vendorId, stream, "image.jpg", "image/jpeg", 3, 1,
                VendorServiceId: state == "empty" ? Guid.Empty : _serviceId), CancellationToken.None);

        response.StatusCode.Should().Be(expectedStatus);
        _storage.VerifyNoOtherCalls();
        _images.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Update_AssignsOrPreservesServiceAndScopesCover(bool assignService)
    {
        var image = new VendorImage
        {
            VendorImageId = Guid.NewGuid(), VendorId = _vendorId,
            VendorServiceId = assignService ? null : _serviceId, ImageUrl = "existing.jpg"
        };
        _images.Setup(x => x.GetByIdAsync(image.VendorImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);

        var handler = CreateUpdateHandler();
        var response = await handler.Handle(
            new UpdateVendorImageCommand(image.VendorImageId, true, 1, true, null, null, null, null,
                assignService ? _serviceId : null), CancellationToken.None);

        response.StatusCode.Should().Be(200);
        response.Data!.VendorServiceId.Should().Be(_serviceId);
        image.VendorServiceId.Should().Be(_serviceId);
        _images.Verify(x => x.ClearOtherCoverImagesAsync(
            _vendorId, image.VendorImageId, It.IsAny<CancellationToken>(), _serviceId), Times.Once);
    }

    [Fact]
    public async Task Update_RejectsServiceFromAnotherVendorWithoutChangingImage()
    {
        SetService(Guid.NewGuid());
        var image = new VendorImage { VendorImageId = Guid.NewGuid(), VendorId = _vendorId };
        _images.Setup(x => x.GetByIdAsync(image.VendorImageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(image);

        var response = await CreateUpdateHandler().Handle(
            new UpdateVendorImageCommand(image.VendorImageId, true, 1, true, null, null, null, null, _serviceId),
            CancellationToken.None);

        response.StatusCode.Should().Be(400);
        image.VendorServiceId.Should().BeNull();
        _unitOfWork.VerifyNoOtherCalls();
        _storage.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task List_FiltersByServiceAndReturnsServiceId()
    {
        var pagination = new PaginationRequest { PageNumber = 1, PageSize = 10 };
        _images.Setup(x => x.GetByVendorIdAsync(_vendorId, pagination, It.IsAny<CancellationToken>(), _serviceId))
            .ReturnsAsync(new PaginatedResult<VendorImage>
            {
                Items = [new VendorImage { VendorId = _vendorId, VendorServiceId = _serviceId, ImageUrl = "image.jpg" }],
                TotalCount = 1, PageNumber = 1, PageSize = 10
            });
        var handler = new GetVendorImagesByVendorIdQueryHandler(_images.Object, _storage.Object, _messages.Object);

        var response = await handler.Handle(
            new GetVendorImagesByVendorIdQuery(_vendorId, pagination, _serviceId), CancellationToken.None);

        response.StatusCode.Should().Be(200);
        response.Data!.Items.Single().VendorServiceId.Should().Be(_serviceId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Cover_UsesRequestedServiceScope(bool serviceCover)
    {
        Guid? serviceId = serviceCover ? _serviceId : null;
        _images.Setup(x => x.GetCoverImageAsync(_vendorId, It.IsAny<CancellationToken>(), serviceId))
            .ReturnsAsync(new VendorImage { VendorId = _vendorId, VendorServiceId = serviceId, ImageUrl = "image.jpg" });
        var handler = new GetVendorCoverImageQueryHandler(_images.Object, _storage.Object, _messages.Object);

        var response = await handler.Handle(new GetVendorCoverImageQuery(_vendorId, serviceId), CancellationToken.None);

        response.StatusCode.Should().Be(200);
        response.Data!.VendorServiceId.Should().Be(serviceId);
        _images.Verify(x => x.GetCoverImageAsync(_vendorId, It.IsAny<CancellationToken>(), serviceId), Times.Once);
    }

    private UpdateVendorImageCommandHandler CreateUpdateHandler()
        => new(_images.Object, _services.Object, _unitOfWork.Object, _storage.Object,
            _publisher.Object, _cache.Object, _messages.Object);

    private void SetService(Guid vendorId, bool active = true, bool deleted = false)
        => _services.Setup(x => x.GetByIdAsync(_serviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VendorService
            {
                VendorServiceId = _serviceId, VendorId = vendorId, IsActive = active, IsDeleted = deleted
            });
}
