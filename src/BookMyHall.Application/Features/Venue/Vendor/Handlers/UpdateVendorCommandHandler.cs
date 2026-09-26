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

public sealed class UpdateVendorCommandHandler(
    IVendorRepository vendorRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateVendorCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService)
    : IRequestHandler<
        UpdateVendorCommand,
        ApiResponse<VendorDto>>
{
    public async Task<ApiResponse<VendorDto>> Handle(
        UpdateVendorCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var vendor =await vendorRepository.GetByIdAsync(request.VendorId,cancellationToken);

        if (vendor is null)
        {
            return ApiResponse<VendorDto>.FailureResponse(messageHelper.NotFound(EntityKeys.Vendor),
                HttpStatusCode.NotFound);
        }

        var businessName =
            request.BusinessName.Trim();

        var existingVendor =
            await vendorRepository
                .GetByBusinessNameIncludingDeletedAsync(
                    businessName,
                    cancellationToken);

        if (existingVendor is not null &&
            existingVendor.VendorId != request.VendorId)
        {
            return ApiResponse<VendorDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.Vendor),
                HttpStatusCode.Conflict);
        }

        mapper.Map(request, vendor);

        vendor.BusinessName = businessName;

        await vendorRepository.UpdateAsync(
            vendor,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        await InvalidateCacheAsync(
            vendor.VendorId,
            cancellationToken);

        return ApiResponse<VendorDto>.SuccessResponse(
            mapper.Map<VendorDto>(vendor),
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.Vendor),
            HttpStatusCode.OK);
    }

    private async Task InvalidateCacheAsync(Guid vendorId, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.Vendors}:{vendorId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorsPaged}:", cancellationToken);
    }
}