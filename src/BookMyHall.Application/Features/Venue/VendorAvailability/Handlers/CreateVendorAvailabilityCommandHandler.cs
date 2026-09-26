using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;
public sealed class CreateVendorAvailabilityCommandHandler(IVendorAvailabilityRepository vendorAvailabilityRepository,
    IVendorRepository vendorRepository,IUnitOfWork unitOfWork,IMapper mapper,
    IValidator<CreateVendorAvailabilityCommand> validator,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateVendorAvailabilityCommand,ApiResponse<VendorAvailabilityDto>>
{
    public async Task<ApiResponse<VendorAvailabilityDto>>Handle(CreateVendorAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorAvailabilityDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var vendor = await vendorRepository.GetByIdAsync(request.VendorId,cancellationToken);
        if (vendor is null)
        {
            return ApiResponse<VendorAvailabilityDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.Vendor),HttpStatusCode.NotFound);
        }

        var isOverlapping =await vendorAvailabilityRepository.ExistsOverlappingAsync(
                request.VendorId,
                request.DayOfWeek,
                request.AvailableDate,
                request.StartTime,
                request.EndTime,
                null,
                cancellationToken);

        if (isOverlapping)
        {
            return ApiResponse<VendorAvailabilityDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
            ResourceNames.Entities,EntityKeys.VendorAvailability),HttpStatusCode.Conflict);
        }
        var vendorAvailability =mapper.Map<VendorAvailability>(request);
        vendorAvailability.VendorAvailabilityId = Guid.NewGuid();
        vendorAvailability.VendorId = request.VendorId;
        vendorAvailability.IsAvailable = true;
        vendorAvailability.IsActive = true;
        vendorAvailability.IsDeleted = false;
        try
        {
            await vendorAvailabilityRepository.AddAsync(vendorAvailability,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorAvailabilityDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
            ResourceNames.Entities,EntityKeys.VendorAvailability),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorAvailability.VendorId,vendorAvailability.VendorAvailabilityId,cancellationToken);
        return ApiResponse<VendorAvailabilityDto>.SuccessResponse(mapper.Map<VendorAvailabilityDto>(vendorAvailability),
        messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorAvailability),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid vendorId,Guid vendorAvailabilityId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorAvailabilities}:{vendorAvailabilityId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorAvailabilitiesPaged}:",cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Vendors}:{vendorId}",cancellationToken);
    }
}