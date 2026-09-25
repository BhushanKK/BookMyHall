using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateVendorCommandHandler(IVendorRepository vendorRepository,IUnitOfWork unitOfWork,
    IMapper mapper,IValidator<CreateVendorCommand> validator,IMessageHelper messageHelper,
    ICacheService cacheService): IRequestHandler<CreateVendorCommand,ApiResponse<VendorDto>>
{
    public async Task<ApiResponse<VendorDto>> Handle(CreateVendorCommand request,CancellationToken cancellationToken)
    {
        var validationResult =await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var businessName =request.BusinessName.Trim();
        var existingVendor=await vendorRepository.GetByBusinessNameIncludingDeletedAsync(businessName,cancellationToken);

        if (existingVendor is not null && !existingVendor.IsDeleted)
        {
            return ApiResponse<VendorDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.Vendor),HttpStatusCode.Conflict);
        }

        if (existingVendor is not null && existingVendor.IsDeleted)
        {
            existingVendor.IsDeleted = false;
            existingVendor.IsActive = true;
            existingVendor.BusinessName = businessName;

            await vendorRepository.UpdateAsync(existingVendor,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingVendor.VendorId,cancellationToken);

            return ApiResponse<VendorDto>.SuccessResponse(mapper.Map<VendorDto>(existingVendor),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.Vendor),HttpStatusCode.Created);
        }

        var vendor = mapper.Map<Vendors>(request);
        vendor.VendorId = Guid.NewGuid();
        vendor.BusinessName = businessName;
        vendor.IsActive = true;
        vendor.IsDeleted = false;

        try
        {
            await vendorRepository.AddAsync(vendor,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.Vendor),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendor.VendorId,cancellationToken);
        return ApiResponse<VendorDto>.SuccessResponse(mapper.Map<VendorDto>(vendor),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.Vendor),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid vendorId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.Vendors}:{vendorId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorsPaged}:",cancellationToken);
    }
}