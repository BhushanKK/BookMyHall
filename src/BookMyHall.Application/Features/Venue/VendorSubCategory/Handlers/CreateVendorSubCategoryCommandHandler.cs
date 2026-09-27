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
public sealed class CreateVendorSubCategoryCommandHandler(IVendorSubCategoryRepository vendorSubCategoryRepository,
    IVendorCategoryRepository vendorCategoryRepository,IUnitOfWork unitOfWork,IMapper mapper,
    IValidator<CreateVendorSubCategoryCommand> validator,IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<CreateVendorSubCategoryCommand,ApiResponse<VendorSubCategoryDto>>
{
    public async Task<ApiResponse<VendorSubCategoryDto>> Handle(CreateVendorSubCategoryCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var vendorCategory =await vendorCategoryRepository.GetByIdAsync(request.VendorCategoryId,cancellationToken);
        if (vendorCategory is null)
        {
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.VendorCategory),HttpStatusCode.NotFound);
        }

        var existingVendorSubCategory =await vendorSubCategoryRepository.GetByNameIncludingDeletedAsync(
                    request.VendorCategoryId,request.Name,cancellationToken);

        if (existingVendorSubCategory is not null &&!existingVendorSubCategory.IsDeleted)
        {
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorSubCategory),HttpStatusCode.Conflict);
        }

        if (existingVendorSubCategory is not null && existingVendorSubCategory.IsDeleted)
        {
            existingVendorSubCategory.Name =request.Name;
            existingVendorSubCategory.Description =request.Description;
            existingVendorSubCategory.DisplayOrder =request.DisplayOrder;
            existingVendorSubCategory.IsActive =request.IsActive;
            existingVendorSubCategory.IsDeleted = false;

            await vendorSubCategoryRepository.UpdateAsync(existingVendorSubCategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await InvalidateCacheAsync(existingVendorSubCategory.VendorSubCategoryId,cancellationToken);
            return ApiResponse<VendorSubCategoryDto>.SuccessResponse(mapper.Map<VendorSubCategoryDto>(existingVendorSubCategory),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorSubCategory),HttpStatusCode.Created);
        }

        var vendorSubCategory =mapper.Map<VendorSubCategory>(request);
        vendorSubCategory.VendorSubCategoryId =Guid.NewGuid();
        vendorSubCategory.VendorCategoryId =request.VendorCategoryId;
        vendorSubCategory.IsActive =request.IsActive;
        vendorSubCategory.IsDeleted = false;
        try
        {
            await vendorSubCategoryRepository.AddAsync(vendorSubCategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorSubCategoryDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorSubCategory),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorSubCategory.VendorSubCategoryId,cancellationToken);

        return ApiResponse<VendorSubCategoryDto>.SuccessResponse(mapper.Map<VendorSubCategoryDto>(vendorSubCategory),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorSubCategory),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid vendorSubCategoryId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorSubCategories}:{vendorSubCategoryId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorSubCategoriesPaged}:",cancellationToken);
    }
}