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
public sealed class CreateVendorServiceAreaCommandHandler(IVendorServiceAreaRepository vendorServiceAreaRepository,
    IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateVendorServiceAreaCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService):
    IRequestHandler<CreateVendorServiceAreaCommand,ApiResponse<VendorServiceAreaDto>>
{
    public async Task<ApiResponse<VendorServiceAreaDto>> Handle(CreateVendorServiceAreaCommand request,CancellationToken cancellationToken)
    {
        var validationResult =await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorServiceAreaDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

          var existingVendorServiceArea =await vendorServiceAreaRepository.GetByLocationIncludingDeletedAsync(
                    request.VendorId,request.StateId,request.CityId,request.AreaId,cancellationToken);

        if (existingVendorServiceArea is not null && !existingVendorServiceArea.IsDeleted)
        {
            return ApiResponse<VendorServiceAreaDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.Conflict);
        }

        if (existingVendorServiceArea is not null && existingVendorServiceArea.IsDeleted)
        {
            existingVendorServiceArea.ServiceRadiusKm = request.ServiceRadiusKm;
            existingVendorServiceArea.IsPrimary=request.IsPrimary;
            existingVendorServiceArea.IsDeleted = false;
            existingVendorServiceArea.IsActive = true;

            await vendorServiceAreaRepository.UpdateAsync(existingVendorServiceArea,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingVendorServiceArea.VendorServiceAreaId,cancellationToken);

            return ApiResponse<VendorServiceAreaDto>.SuccessResponse(mapper.Map<VendorServiceAreaDto>(existingVendorServiceArea),
                messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.Created);
        }

        var vendorServiceArea = mapper.Map<VendorServiceArea>(request);
        vendorServiceArea.VendorServiceAreaId = Guid.NewGuid();
        vendorServiceArea.IsDeleted = false;
        try
        {
            await vendorServiceAreaRepository.AddAsync(vendorServiceArea,cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorServiceAreaDto>.FailureResponse(messageHelper.AlreadyExistsEntity(
                    ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorServiceArea.VendorServiceAreaId,cancellationToken);
        return ApiResponse<VendorServiceAreaDto>.SuccessResponse(mapper.Map<VendorServiceAreaDto>(vendorServiceArea),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.VendorServiceArea),HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid vendorServiceAreaId,CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorServiceAreas}:{vendorServiceAreaId}",cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorServiceAreasPaged}:",cancellationToken);
    }
}