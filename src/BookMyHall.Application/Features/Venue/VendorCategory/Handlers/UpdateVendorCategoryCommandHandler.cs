using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Caching;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence.Repositories;

namespace BookMyHall.Application.Features.Venue;

public sealed class UpdateVendorCategoryCommandHandler(IVendorCategoryRepository vendorCategoryRepository,IUnitOfWork unitOfWork,
    IMapper mapper,IValidator<UpdateVendorCategoryCommand> validator,IMessageHelper messageHelper,
    ICacheService cacheService): IRequestHandler<UpdateVendorCategoryCommand,ApiResponse<VendorCategoryDto>>
{
    public async Task<ApiResponse<VendorCategoryDto>> Handle(UpdateVendorCategoryCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorCategoryDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var vendorcategory =await vendorCategoryRepository.GetByIdAsync(request.VendorCategoryId,cancellationToken);

        if (vendorcategory is null)
        {
            return ApiResponse<VendorCategoryDto>.FailureResponse(messageHelper.NotFound
            (EntityKeys.VendorCategory),HttpStatusCode.NotFound);
        }

        var name =request.Name.Trim();
        var existingVendorCategory =await vendorCategoryRepository.GetByNameIncludingDeletedAsync(name,cancellationToken);

        if (existingVendorCategory is not null && existingVendorCategory.VendorCategoryId != request.VendorCategoryId)
        {
            return ApiResponse<VendorCategoryDto>.FailureResponse(messageHelper.AlreadyExistsEntity
            (ResourceNames.Entities,EntityKeys.VendorCategory),HttpStatusCode.Conflict);
        }

        mapper.Map(request, vendorcategory);
        vendorcategory.Name = name;
        await vendorCategoryRepository.UpdateAsync(vendorcategory,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await InvalidateCacheAsync(vendorcategory.VendorCategoryId,cancellationToken);
        return ApiResponse<VendorCategoryDto>.SuccessResponse(mapper.Map<VendorCategoryDto>(vendorcategory),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.Vendor),HttpStatusCode.OK);
    }

    private async Task InvalidateCacheAsync(Guid vendorCategoryId, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorCategories}:{vendorCategoryId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorCategoriesPaged}:", cancellationToken);
    }
}