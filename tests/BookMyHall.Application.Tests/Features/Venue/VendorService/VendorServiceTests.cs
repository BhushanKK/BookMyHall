using Moq;
using System.Text.Json;
using AutoMapper;
using BookMyHall.Application.Features.Venue;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Shared.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Tests.Features.Venue.VendorService;

public sealed class VendorServiceTests
{
    private readonly Mock<IVendorServiceRepository> _services = new();
    private readonly Mock<IVendorRepository> _vendors = new();
    private readonly Mock<IVendorSubCategoryRepository> _subCategories = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IMessageHelper> _messages = new();
    private readonly IMapper _mapper = new MapperConfiguration(config => config.AddProfile<VendorServiceMappingProfile>(), NullLoggerFactory.Instance).CreateMapper();
    private readonly Vendor _vendor = new() { VendorId = Guid.NewGuid(), UserId = Guid.NewGuid(), BusinessName = "Wedding Services", IsActive = true };
    private readonly VendorSubCategory _subCategory = new()
    {
        VendorSubCategoryId = Guid.NewGuid(), VendorCategoryId = Guid.NewGuid(), VendorSubCategoryName = "Bridal Mehandi", IsActive = true,
        VendorCategory = new VendorCategory { VendorCategoryName = "Mehandi" }
    };

    public VendorServiceTests()
    {
        _vendors.Setup(x => x.GetByIdAsync(_vendor.VendorId, It.IsAny<CancellationToken>())).ReturnsAsync(_vendor);
        _subCategories.Setup(x => x.GetByIdAsync(_subCategory.VendorSubCategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(_subCategory);
    }

    private CreateVendorServiceCommand Command(string name = "Bridal Mehandi") => new()
    {
        VendorId = _vendor.VendorId, UserId = Guid.NewGuid(), VendorCategoryId = _subCategory.VendorCategoryId,
        VendorSubCategoryId = _subCategory.VendorSubCategoryId, ServiceName = name, IsActive = true
    };

    private CreateVendorServiceCommandHandler CreateHandler() => new(_services.Object, _vendors.Object, _subCategories.Object,
        _unitOfWork.Object, _mapper, new CreateVendorServiceCommandValidator(), _messages.Object, _cache.Object);

    [Fact]
    public async Task OneVendorCanOfferMultipleServicesAcrossCategories()
    {
        var saved = new List<BookMyHall.Domain.Venue.VendorService>();
        _services.Setup(x => x.AddAsync(It.IsAny<BookMyHall.Domain.Venue.VendorService>(), It.IsAny<CancellationToken>()))
            .Callback<BookMyHall.Domain.Venue.VendorService, CancellationToken>((service, _) => saved.Add(service)).Returns(Task.CompletedTask);
        var bridal = await CreateHandler().Handle(Command(), CancellationToken.None);
        _subCategory.VendorSubCategoryId = Guid.NewGuid();
        _subCategory.VendorSubCategoryName = "Guest Mehandi";
        _subCategories.Setup(x => x.GetByIdAsync(_subCategory.VendorSubCategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(_subCategory);
        var guests = await CreateHandler().Handle(Command("Guest Mehandi"), CancellationToken.None);
        _subCategory.VendorCategoryId = Guid.NewGuid();
        _subCategory.VendorSubCategoryId = Guid.NewGuid();
        _subCategory.VendorSubCategoryName = "Wedding Catering";
        _subCategories.Setup(x => x.GetByIdAsync(_subCategory.VendorSubCategoryId, It.IsAny<CancellationToken>())).ReturnsAsync(_subCategory);
        var catering = await CreateHandler().Handle(Command("Wedding Catering"), CancellationToken.None);

        bridal.Success.Should().BeTrue();
        guests.Success.Should().BeTrue();
        catering.Success.Should().BeTrue();
        saved.Should().HaveCount(3);
        saved.Select(x => x.VendorServiceId).Should().OnlyHaveUniqueItems();
        saved.Should().OnlyContain(x => x.VendorId == _vendor.VendorId && x.UserId == _vendor.UserId);
        saved.Select(x => x.VendorCategoryId).Distinct().Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateDerivesOwnerAndTrimsName()
    {
        var response = await CreateHandler().Handle(Command("  Bridal Mehandi  "), CancellationToken.None);
        response.Data!.UserId.Should().Be(_vendor.UserId!.Value);
        response.Data.ServiceName.Should().Be("Bridal Mehandi");
        response.Data.BusinessName.Should().Be(_vendor.BusinessName);
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(response.Data, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        json.RootElement.GetProperty("vendorServiceId").GetGuid().Should().NotBeEmpty();
        json.RootElement.TryGetProperty("vendor", out _).Should().BeFalse();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MismatchedCategoryIsRejectedBeforeSaving(bool update)
    {
        var command = Command();
        command.VendorCategoryId = Guid.NewGuid();
        ApiResponse<VendorServiceDto> response;
        if (update)
        {
            var serviceId = Guid.NewGuid();
            _services.Setup(x => x.GetByIdAsync(serviceId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BookMyHall.Domain.Venue.VendorService { VendorServiceId = serviceId, VendorId = _vendor.VendorId });
            var handler = new UpdateVendorServiceCommandHandler(_services.Object, _vendors.Object, _subCategories.Object,
                _unitOfWork.Object, _mapper, new UpdateVendorServiceCommandValidator(), _messages.Object, _cache.Object);
            response = await handler.Handle(new UpdateVendorServiceCommand
            {
                VendorServiceId = serviceId, VendorId = command.VendorId, VendorCategoryId = command.VendorCategoryId,
                VendorSubCategoryId = command.VendorSubCategoryId, ServiceName = command.ServiceName
            }, CancellationToken.None);
        }
        else response = await CreateHandler().Handle(command, CancellationToken.None);

        response.StatusCode.Should().Be(400);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DuplicateServiceIsRejectedButDifferentServiceIsAllowed()
    {
        _services.Setup(x => x.GetByNameIncludingDeletedAsync(_vendor.VendorId, "Bridal Mehandi", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookMyHall.Domain.Venue.VendorService { VendorId = _vendor.VendorId });

        (await CreateHandler().Handle(Command(), CancellationToken.None)).StatusCode.Should().Be(409);
        (await CreateHandler().Handle(Command("Guest Mehandi"), CancellationToken.None)).Success.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateCannotMoveExistingServiceToAnotherVendor()
    {
        var id = Guid.NewGuid();
        _services.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(
            new BookMyHall.Domain.Venue.VendorService { VendorServiceId = id, VendorId = Guid.NewGuid() });
        var handler = new UpdateVendorServiceCommandHandler(_services.Object, _vendors.Object, _subCategories.Object,
            _unitOfWork.Object, _mapper, new UpdateVendorServiceCommandValidator(), _messages.Object, _cache.Object);
        var response = await handler.Handle(new UpdateVendorServiceCommand
        {
            VendorServiceId = id, VendorId = _vendor.VendorId, VendorCategoryId = _subCategory.VendorCategoryId,
            VendorSubCategoryId = _subCategory.VendorSubCategoryId, ServiceName = "Bridal Mehandi"
        }, CancellationToken.None);

        response.StatusCode.Should().Be(400);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ListCacheSeparatesAllVendorAndCategoryFilters()
    {
        var keys = new List<string>();
        _cache.Setup(x => x.GetAsync<PaginatedResponse<VendorServiceDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((key, _) => keys.Add(key)).ReturnsAsync((PaginatedResponse<VendorServiceDto>?)null);
        _services.Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedResult<BookMyHall.Domain.Venue.VendorService> { Items = [], TotalCount = 0 });
        var handler = new GetVendorServicesQueryHandler(_services.Object, _mapper, _messages.Object, _cache.Object);
        var pagination = new PaginationRequest();
        var vendorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var subCategoryId = Guid.NewGuid();
        await handler.Handle(new(pagination, null, null, null), CancellationToken.None);
        await handler.Handle(new(pagination, vendorId, null, null), CancellationToken.None);
        await handler.Handle(new(pagination, vendorId, categoryId, null), CancellationToken.None);
        await handler.Handle(new(pagination, vendorId, categoryId, subCategoryId), CancellationToken.None);

        keys.Should().HaveCount(4).And.OnlyHaveUniqueItems();
    }
}
