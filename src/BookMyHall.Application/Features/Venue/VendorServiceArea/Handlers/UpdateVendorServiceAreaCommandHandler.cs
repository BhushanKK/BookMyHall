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

public sealed class UpdateVendorServiceAreaCommandHandler(IVendorServiceAreaRepository vendorServiceAreaRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<UpdateVendorServiceAreaCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<UpdateVendorServiceAreaCommand,ApiResponse<VendorServiceAreaDto>>
{
    public async Task<ApiResponse<VendorServiceAreaDto>> Handle(UpdateVendorServiceAreaCommand request,CancellationToken cancellationToken)
    {
        var validationResult =await validator.ValidateAsync(request,cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorServiceAreaDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var vendorServiceArea =await vendorServiceAreaRepository.GetByIdAsync(request.VendorServiceAreaId,cancellationToken);
        if (vendorServiceArea is null)
        {
            return ApiResponse<VendorServiceAreaDto>.FailureResponse(messageHelper.NotFoundEntity(
                        ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.NotFound);
        }

        var exists =await vendorServiceAreaRepository.ExistsAsync(
                request.VendorId,
                request.StateId,
                request.CityId,
                request.AreaId,
                request.VendorServiceAreaId,
                cancellationToken);

        if (exists)
        {
            return ApiResponse<VendorServiceAreaDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                        ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.Conflict);
        }

        mapper.Map(request, vendorServiceArea);
        vendorServiceArea.IsDeleted = false;

        try
        {
            await vendorServiceAreaRepository.UpdateAsync(vendorServiceArea,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorServiceAreaDto>.FailureResponse(messageHelper.AlreadyExistsEntity(ResourceNames.Entities,
                        EntityKeys.VendorServiceArea),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorServiceArea.VendorServiceAreaId,cancellationToken);
        return ApiResponse<VendorServiceAreaDto>.SuccessResponse(mapper.Map<VendorServiceAreaDto>(vendorServiceArea),
                messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.OK);
    }

    private async Task InvalidateCacheAsync(Guid vendorServiceAreaId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorServiceAreas}:{vendorServiceAreaId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorServiceAreasPaged}:",cancellationToken);
    }
}