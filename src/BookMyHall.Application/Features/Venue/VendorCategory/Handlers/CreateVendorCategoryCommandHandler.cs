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
public sealed class CreateVendorCategoryCommandHandler(IVendorCategoryRepository vendorCategoryRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateVendorCategoryCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService):
    IRequestHandler<CreateVendorCategoryCommand,ApiResponse<VendorCategoryDto>>
{
    public async Task<ApiResponse<VendorCategoryDto>> Handle(CreateVendorCategoryCommand request,CancellationToken cancellationToken)
    {
        var validationResult =await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorCategoryDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var name =request.Name.Trim();
        var existingVendorCategory=await vendorCategoryRepository.GetByNameIncludingDeletedAsync(name,cancellationToken);

        if (existingVendorCategory is not null && !existingVendorCategory.IsDeleted)
        {
            return ApiResponse<VendorCategoryDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorCategory),HttpStatusCode.Conflict);
        }

        if (existingVendorCategory is not null && existingVendorCategory.IsDeleted)
        {
            existingVendorCategory.IsDeleted = false;
            existingVendorCategory.IsActive = true;
            existingVendorCategory.Name = name;

            await vendorCategoryRepository.UpdateAsync(existingVendorCategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingVendorCategory.VendorCategoryId,cancellationToken);

            return ApiResponse<VendorCategoryDto>.SuccessResponse(mapper.Map<VendorCategoryDto>(existingVendorCategory),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorCategory),HttpStatusCode.Created);
        }

        var vendorcategory = mapper.Map<VendorCategory>(request);
        vendorcategory.VendorCategoryId = Guid.NewGuid();
        vendorcategory.Name = name;
        vendorcategory.IsActive = true;
        vendorcategory.IsDeleted = false;

        try
        {
            await vendorCategoryRepository.AddAsync(vendorcategory,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorCategoryDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorCategory),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorcategory.VendorCategoryId,cancellationToken);
        return ApiResponse<VendorCategoryDto>.SuccessResponse(mapper.Map<VendorCategoryDto>(vendorcategory),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorCategory),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid vendorcategoryId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorCategories}:{vendorcategoryId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorCategoriesPaged}:",cancellationToken);
    }
}