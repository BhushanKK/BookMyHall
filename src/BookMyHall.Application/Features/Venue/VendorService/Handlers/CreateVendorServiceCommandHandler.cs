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

public sealed class CreateVendorServiceCommandHandler(IVendorServiceRepository vendorServiceRepository,
    IVendorRepository vendorRepository,IVendorSubCategoryRepository vendorSubCategoryRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateVendorServiceCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateVendorServiceCommand,ApiResponse<VendorServiceDto>>
{
    public async Task<ApiResponse<VendorServiceDto>> Handle(CreateVendorServiceCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorServiceDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var vendor = await vendorRepository.GetByIdAsync(request.VendorId,cancellationToken);

        if (vendor is null)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.Vendor),HttpStatusCode.NotFound);
        }

        var vendorSubCategory =await vendorSubCategoryRepository.GetByIdAsync(request.VendorSubCategoryId,cancellationToken);

        if (vendorSubCategory is null)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.VendorSubCategory),HttpStatusCode.NotFound);
        }

        var existingVendorService =await vendorServiceRepository.GetByNameIncludingDeletedAsync(request.VendorId,
                    request.ServiceName,
                    cancellationToken);

        if (existingVendorService is not null &&!existingVendorService.IsDeleted)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.Conflict);
        }

        if (existingVendorService is not null && existingVendorService.IsDeleted)
        {
            existingVendorService.VendorSubCategoryId =request.VendorSubCategoryId;
            existingVendorService.Description =request.Description;
            existingVendorService.PricingType =request.PricingType;
            existingVendorService.BasePrice =request.BasePrice;
            existingVendorService.MinPrice =request.MinPrice;
            existingVendorService.MaxPrice =request.MaxPrice;
            existingVendorService.UnitName =request.UnitName;
            existingVendorService.MinimumQuantity =request.MinimumQuantity;
            existingVendorService.MaximumQuantity =request.MaximumQuantity;
            existingVendorService.IsPackage =request.IsPackage;
            existingVendorService.IsActive =request.IsActive;
            existingVendorService.IsDeleted = false;

            await vendorServiceRepository.UpdateAsync(existingVendorService,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(existingVendorService.VendorId,existingVendorService.VendorServiceId,cancellationToken);
            return ApiResponse<VendorServiceDto>.SuccessResponse(mapper.Map<VendorServiceDto>(existingVendorService),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.Created);
        }

        var vendorService = mapper.Map<VendorService>(request);
        vendorService.VendorServiceId = Guid.NewGuid();
        vendorService.VendorId = request.VendorId;
        vendorService.VendorSubCategoryId =request.VendorSubCategoryId;
        vendorService.IsActive = request.IsActive;
        vendorService.IsDeleted = false;

        try
        {
            await vendorServiceRepository.AddAsync(vendorService,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorServiceDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorService.VendorId,vendorService.VendorServiceId,cancellationToken);
        return ApiResponse<VendorServiceDto>.SuccessResponse(mapper.Map<VendorServiceDto>(vendorService),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid vendorId,Guid vendorServiceId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorServices}:{vendorServiceId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorServicesPaged}:",cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Vendors}:{vendorId}",cancellationToken);
    }
}