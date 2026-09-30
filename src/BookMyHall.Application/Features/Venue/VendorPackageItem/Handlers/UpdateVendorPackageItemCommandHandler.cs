using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;
public sealed class UpdateVendorPackageItemCommandHandler(IVendorPackageItemRepository vendorPackageItemRepository,IUnitOfWork unitOfWork,
    IMapper mapper,IValidator<UpdateVendorPackageItemCommand> validator,IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<UpdateVendorPackageItemCommand, ApiResponse<VendorPackageItemDto>>
{
    public async Task<ApiResponse<VendorPackageItemDto>> Handle(UpdateVendorPackageItemCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorPackageItemDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var vendorpackageitem = await vendorPackageItemRepository.GetByIdAsync(request.VendorPackageItemId, cancellationToken);
        if (vendorpackageitem is null)
        {
            return ApiResponse<VendorPackageItemDto>.FailureResponse(messageHelper.NotFound(EntityKeys.VendorPackageItem),
                HttpStatusCode.NotFound);
        }

        var exists = await vendorPackageItemRepository.ExistsAsync(request.VendorPackageId,request.VendorServiceId,request.VendorPackageItemId, cancellationToken);

        if (exists)
        {
            return ApiResponse<VendorPackageItemDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                        ResourceNames.Entities,EntityKeys.VendorPackageItem),HttpStatusCode.Conflict);
        }

        mapper.Map(request, vendorpackageitem);
        vendorpackageitem.IsDeleted=false;
        await vendorPackageItemRepository.UpdateAsync(vendorpackageitem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        var cacheKey = $"{CacheKeys.VendorPackageItems}:{request.VendorPackageItemId}";
        await cacheService.RemoveAsync(cacheKey, cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorPackageItemsPaged}:", cancellationToken);

        return ApiResponse<VendorPackageItemDto>.SuccessResponse(mapper.Map<VendorPackageItemDto>(vendorpackageitem),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.VendorPackageItem), HttpStatusCode.OK);
    }
}