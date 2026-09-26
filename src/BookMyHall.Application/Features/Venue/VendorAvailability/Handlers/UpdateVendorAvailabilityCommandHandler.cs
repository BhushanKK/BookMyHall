using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateVendorAvailabilityCommandHandler(
    IVendorAvailabilityRepository repository,
    IVendorRepository vendorRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateVendorAvailabilityCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        UpdateVendorAvailabilityCommand,
        ApiResponse<VendorAvailabilityDto>>
{
    public async Task<ApiResponse<VendorAvailabilityDto>> Handle(
        UpdateVendorAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Fluent Validation Check
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorAvailabilityDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        // 2. Main Availability Record Check
        var entity = await repository.GetByIdAsync(
            request.VendorAvailabilityId,
            cancellationToken);

        if (entity is null)
        {
            return ApiResponse<VendorAvailabilityDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.VendorAvailability),
                HttpStatusCode.NotFound);
        }

        // 3. Parent Vendor Reference Validation Check
        var vendor = await vendorRepository.GetByIdAsync(
            request.VendorId,
            cancellationToken);

        if (vendor is null)
        {
            return ApiResponse<VendorAvailabilityDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Vendor),
                HttpStatusCode.NotFound);
        }

        // 4. Overlap Business Rule Validation Check
        var overlap = await repository.ExistsOverlappingAsync(
            request.VendorId,
            request.DayOfWeek,
            request.AvailableDate,
            request.StartTime,
            request.EndTime,
            request.VendorAvailabilityId,
            cancellationToken);

        if (overlap)
        {
            return ApiResponse<VendorAvailabilityDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.VendorAvailability), // Or appropriate overlap constant key
                HttpStatusCode.Conflict);
        }

        // 5. Data Mapping & Standardization Layout
        mapper.Map(request, entity);

        entity.Reason = string.IsNullOrWhiteSpace(request.Reason)
            ? null
            : request.Reason.Trim();

        // 6. DB Updates & Transaction Processing
        await repository.UpdateAsync(
            entity,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // 7. Dynamic Memory Cache Invalidation Handling
        await InvalidateCacheAsync(
            request.VendorId,
            cancellationToken);

        // 8. Structured Standardized Success Output Response
        return ApiResponse<VendorAvailabilityDto>.SuccessResponse(
            mapper.Map<VendorAvailabilityDto>(entity),
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.VendorAvailability),
            HttpStatusCode.OK);
    }

    private async Task InvalidateCacheAsync(Guid vendorId, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorAvailabilities}:{vendorId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorAvailabilitiesPaged}:", cancellationToken);
    }
}
