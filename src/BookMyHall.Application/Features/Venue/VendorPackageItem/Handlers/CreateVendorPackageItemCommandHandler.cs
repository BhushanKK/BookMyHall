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

public sealed class CreateVendorPackageItemCommandHandler(IVendorPackageItemRepository vendorPackageItemRepository,
    IVendorPackageRepository vendorPackageRepository,IVendorServiceRepository vendorServiceRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateVendorPackageItemCommand> validator,
    IMessageHelper messageHelper,ICacheService cachePackageItem)
    : IRequestHandler<CreateVendorPackageItemCommand,ApiResponse<VendorPackageItemDto>>
{
    public async Task<ApiResponse<VendorPackageItemDto>> Handle(CreateVendorPackageItemCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorPackageItemDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var vendorPackage = await vendorPackageRepository.GetByIdAsync(request.VendorPackageId,cancellationToken);

        if (vendorPackage is null)
        {
            return ApiResponse<VendorPackageItemDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.VendorPackage),HttpStatusCode.NotFound);
        }

        var vendorService =await vendorServiceRepository.GetByIdAsync(request.VendorServiceId,cancellationToken);

        if (vendorService is null)
        {
            return ApiResponse<VendorPackageItemDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.VendorService),HttpStatusCode.NotFound);
        }

        var existingVendorPackageItem =await vendorPackageItemRepository.GetByPackageAndServiceIncludingDeletedAsync(request.VendorPackageId,
                    request.VendorServiceId,
                    cancellationToken);

        if (existingVendorPackageItem is not null &&!existingVendorPackageItem.IsDeleted)
        {
            return ApiResponse<VendorPackageItemDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorPackageItem),HttpStatusCode.Conflict);
        }

        if (existingVendorPackageItem is not null && existingVendorPackageItem.IsDeleted)
        {
            existingVendorPackageItem.Quantity  =request.Quantity ;
            existingVendorPackageItem.DisplayOrder =request.DisplayOrder;
            existingVendorPackageItem.IsDeleted = false;

            await vendorPackageItemRepository.UpdateAsync(existingVendorPackageItem,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(existingVendorPackageItem.VendorPackageId,existingVendorPackageItem.VendorPackageItemId,cancellationToken);
            return ApiResponse<VendorPackageItemDto>.SuccessResponse(mapper.Map<VendorPackageItemDto>(existingVendorPackageItem),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorPackageItem),HttpStatusCode.Created);
        }

        var vendorPackageItem = mapper.Map<VendorPackageItem>(request);
        vendorPackageItem.VendorPackageItemId = Guid.NewGuid();
        vendorPackageItem.IsDeleted = false;

        try
        {
            await vendorPackageItemRepository.AddAsync(vendorPackageItem,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorPackageItemDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorPackageItem),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorPackageItem.VendorPackageId,vendorPackageItem.VendorPackageItemId,cancellationToken);
        return ApiResponse<VendorPackageItemDto>.SuccessResponse(mapper.Map<VendorPackageItemDto>(vendorPackageItem),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorPackageItem),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid vendorPackageId,Guid vendorPackageItemId,CancellationToken cancellationToken)
    {
        await cachePackageItem.RemoveAsync($"{CacheKeys.VendorPackageItems}:{vendorPackageItemId}",cancellationToken);
        await cachePackageItem.RemoveByPrefixAsync($"{CacheKeys.VendorPackageItemsPaged}:",cancellationToken);
        await cachePackageItem.RemoveAsync($"{CacheKeys.VendorPackages}:{vendorPackageId}",cancellationToken);
    }
}