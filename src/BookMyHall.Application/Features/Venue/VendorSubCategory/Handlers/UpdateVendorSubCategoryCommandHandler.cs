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
public sealed class UpdateVendorSubCategoryCommandHandler(IVendorSubCategoryRepository vendorSubCategoryRepository,
    IVendorCategoryRepository vendorCategoryRepository,IUnitOfWork unitOfWork,IMapper mapper,
    IValidator<UpdateVendorSubCategoryCommand> validator,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<UpdateVendorSubCategoryCommand,ApiResponse<VendorSubCategoryDto>>
{
    public async Task<ApiResponse<VendorSubCategoryDto>> Handle(UpdateVendorSubCategoryCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }
        var vendorSubCategory =await vendorSubCategoryRepository.GetByIdAsync(request.VendorSubCategoryId,cancellationToken);

        if (vendorSubCategory is null)
        {
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.VendorSubCategory),
                HttpStatusCode.NotFound);
        }

        var vendorCategory =await vendorCategoryRepository.GetByIdAsync(request.VendorCategoryId,cancellationToken);

        if (vendorCategory is null)
        {
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(
                messageHelper.NotFoundEntity(
                    ResourceNames.Entities,
                    EntityKeys.VendorCategory),
                HttpStatusCode.NotFound);
        }

        var existingVendorSubCategory =await vendorSubCategoryRepository.GetByNameAsync(request.VendorCategoryId,
                request.Name,cancellationToken);

        if (existingVendorSubCategory is not null && existingVendorSubCategory.VendorSubCategoryId !=request.VendorSubCategoryId)
        {
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.VendorSubCategory),
                HttpStatusCode.Conflict);
        }

        vendorSubCategory.VendorCategoryId =request.VendorCategoryId;
        vendorSubCategory.Name = request.Name;
        vendorSubCategory.Description =request.Description;
        vendorSubCategory.DisplayOrder =request.DisplayOrder;
        vendorSubCategory.IsActive =request.IsActive;
        try
        {
            await vendorSubCategoryRepository.UpdateAsync(vendorSubCategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,
                    EntityKeys.VendorSubCategory),
                HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorSubCategory.VendorSubCategoryId,cancellationToken);

        return ApiResponse<VendorSubCategoryDto>.SuccessResponse(
            mapper.Map<VendorSubCategoryDto>(
                vendorSubCategory),
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.VendorSubCategory),
            HttpStatusCode.OK);
    }

    private async Task InvalidateCacheAsync(Guid vendorSubCategoryId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorSubCategories}:{vendorSubCategoryId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync( $"{CacheKeys.VendorSubCategoriesPaged}:",cancellationToken);
    }
}