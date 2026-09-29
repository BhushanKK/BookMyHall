using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateVendorPackageCommandHandler(IVendorPackageRepository repository,
    IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateVendorPackageCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<CreateVendorPackageCommand, ApiResponse<VendorPackageDto>>
{
    public async Task<ApiResponse<VendorPackageDto>> Handle(CreateVendorPackageCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorPackageDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var packageName = request.PackageName.Trim();
        var existingPackage = await repository.GetByNameIncludingDeletedAsync(packageName, request.VendorId, cancellationToken);

        if (existingPackage is not null && !existingPackage.IsDeleted)
        {
            return ApiResponse<VendorPackageDto>.FailureResponse(messageHelper.AlreadyExistsEntity
            (ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.Conflict);
        }

        if (existingPackage is not null && existingPackage.IsDeleted)
        {
            existingPackage.IsDeleted = false;
            existingPackage.IsActive = true;
            existingPackage.PackageName = packageName;
            existingPackage.Price = request.Price;
            existingPackage.Description = request.Description;

            await repository.UpdateAsync(existingPackage, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await InvalidateCacheAsync(existingPackage.VendorPackageId, cancellationToken);

            return ApiResponse<VendorPackageDto>.SuccessResponse(mapper.Map<VendorPackageDto>(existingPackage),
                messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.Created);
        }

        var vendorPackage = mapper.Map<VendorPackage>(request);
        vendorPackage.VendorPackageId = Guid.NewGuid();
        vendorPackage.PackageName = packageName;
        vendorPackage.IsActive = true;
        vendorPackage.IsDeleted = false;

        try
        {
            await repository.AddAsync(vendorPackage, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<VendorPackageDto>.FailureResponse(
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.Conflict);
        }

        await InvalidateCacheAsync(vendorPackage.VendorPackageId, cancellationToken);
        return ApiResponse<VendorPackageDto>.SuccessResponse(mapper.Map<VendorPackageDto>(vendorPackage),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.Created);
    }

    private async Task InvalidateCacheAsync(Guid id, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.VendorPackages}:{id}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorPackagesPaged}:", cancellationToken);
    }
}
