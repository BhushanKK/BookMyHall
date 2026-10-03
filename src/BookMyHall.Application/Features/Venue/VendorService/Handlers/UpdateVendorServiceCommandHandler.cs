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

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateVendorServiceCommandHandler(IVendorServiceRepository vendorServiceRepository,
    IVendorRepository vendorRepository,IVendorSubCategoryRepository vendorSubCategoryRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<UpdateVendorServiceCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<UpdateVendorServiceCommand,ApiResponse<VendorServiceDto>>
{
    public async Task<ApiResponse<VendorServiceDto>> Handle(UpdateVendorServiceCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorServiceDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var vendorService =await vendorServiceRepository.GetByIdAsync(request.VendorServiceId,cancellationToken);

        if (vendorService is null)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse( messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.NotFound);
        }

        if (vendorService.VendorId != request.VendorId)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse("A service cannot be moved to another vendor. Create a separate service for that vendor.", HttpStatusCode.BadRequest);
        }

        var vendor = await vendorRepository.GetByIdAsync(request.VendorId,cancellationToken);

        if (vendor is null)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.Vendor),HttpStatusCode.NotFound);
        }

        var vendorSubCategory = await vendorSubCategoryRepository.GetByIdAsync(request.VendorSubCategoryId,cancellationToken);

        if (vendorSubCategory is null)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.VendorSubCategory),HttpStatusCode.NotFound);
        }

        if (vendorSubCategory.VendorCategoryId != request.VendorCategoryId)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse("The subcategory does not belong to the selected category.", HttpStatusCode.BadRequest);
        }
        if (!vendor.UserId.HasValue || vendor.UserId == Guid.Empty)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse("The vendor must have an owner.", HttpStatusCode.BadRequest);
        }
        request.UserId = vendor.UserId.Value;
        request.ServiceName = request.ServiceName.Trim();

        var existingVendorService = await vendorServiceRepository.GetByNameAsync(request.VendorId,request.ServiceName,cancellationToken);

        if (existingVendorService is not null && existingVendorService.VendorServiceId !=request.VendorServiceId)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.Conflict);
        }

        vendorService.VendorId =request.VendorId;
        vendorService.Vendor = vendor;
        vendorService.VendorSubCategory = vendorSubCategory;
        vendorService.VendorSubCategoryId =request.VendorSubCategoryId;
        vendorService.UserId =request.UserId;
        vendorService.VendorCategoryId =request.VendorCategoryId;
        vendorService.ServiceName =
            request.ServiceName;

        vendorService.Description =
            request.Description;

        vendorService.PricingType =
            request.PricingType;

        vendorService.BasePrice =
            request.BasePrice;

        vendorService.MinPrice =
            request.MinPrice;

        vendorService.MaxPrice =
            request.MaxPrice;

        vendorService.UnitName =
            request.UnitName;

        vendorService.MinimumQuantity =
            request.MinimumQuantity;

        vendorService.MaximumQuantity =
            request.MaximumQuantity;

        vendorService.IsPackage =
            request.IsPackage;

        vendorService.IsActive =
            request.IsActive;

        try
        {
            await vendorServiceRepository.UpdateAsync(vendorService,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorService.VendorId,vendorService.VendorServiceId,cancellationToken);
        return ApiResponse<VendorServiceDto>.SuccessResponse(mapper.Map<VendorServiceDto>(vendorService),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.OK);
    }

    private async Task InvalidateCacheAsync(Guid vendorId,Guid vendorServiceId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorServices}:{vendorServiceId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorServicesPaged}:",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorServicesAutoComplete}:",cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Vendors}:{vendorId}",cancellationToken);
    }
}
