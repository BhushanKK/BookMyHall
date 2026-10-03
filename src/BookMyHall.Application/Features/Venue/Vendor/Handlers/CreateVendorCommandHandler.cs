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
using BookMyHall.Application.Common.Interfaces.Storage;
using Microsoft.Extensions.Logging;

namespace BookMyHall.Application.Features.Venue;

public sealed class CreateVendorCommandHandler(
    IVendorRepository vendorRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateVendorCommand> validator,
    IMessageHelper messageHelper,
    ICacheService cacheService,
    IR2StorageService storage,
    ILogger<CreateVendorCommandHandler> logger)
    : IRequestHandler<CreateVendorCommand, ApiResponse<VendorDto>>
{
    public async Task<ApiResponse<VendorDto>> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var businessName = request.BusinessName.Trim();
        var logoError = VendorLogoStorage.Validate(request.Logo);
        if (logoError is not null)
        {
            return ApiResponse<VendorDto>.FailureResponse(logoError, HttpStatusCode.BadRequest);
        }

        var existingVendor = await vendorRepository.GetByBusinessNameIncludingDeletedAsync(businessName, cancellationToken);

        if (existingVendor is not null && !existingVendor.IsDeleted)
        {
            return ApiResponse<VendorDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.Vendor),
                HttpStatusCode.Conflict
            );
        }

        var vendor = existingVendor ?? mapper.Map<Vendor>(request);
        if (existingVendor is null)
        {
            vendor.VendorId = Guid.NewGuid();
        }
        var previousLogo = vendor.LogoUrl;
        var previouslyDeleted = vendor.IsDeleted;
        string? uploadedLogo = null;
        vendor.BusinessName = businessName;
        vendor.IsActive = true;
        vendor.IsDeleted = false;

        try
        {
            if (request.Logo is not null)
            {
                uploadedLogo = await VendorLogoStorage.UploadAsync(storage, vendor.VendorId, request.Logo, logger, cancellationToken);
                vendor.LogoUrl = uploadedLogo;
            }
            if (existingVendor is null)
            {
                await vendorRepository.AddAsync(vendor, cancellationToken);
            }
            else
            {
                await vendorRepository.UpdateAsync(vendor, cancellationToken);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            vendor.LogoUrl = previousLogo;
            vendor.IsDeleted = previouslyDeleted;
            await VendorLogoStorage.DeleteSafelyAsync(storage, uploadedLogo, logger);
            return ApiResponse<VendorDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities, EntityKeys.Vendor),
                HttpStatusCode.Conflict
            );
        }
        catch
        {
            vendor.LogoUrl = previousLogo;
            vendor.IsDeleted = previouslyDeleted;
            await VendorLogoStorage.DeleteSafelyAsync(storage, uploadedLogo, logger);
            throw;
        }

        if (uploadedLogo is not null)
        {
            await VendorLogoStorage.DeleteSafelyAsync(storage, previousLogo, logger);
        }

        await InvalidateCacheAsync(vendor.VendorId, cancellationToken);

        return ApiResponse<VendorDto>.SuccessResponse
        (
            mapper.Map<VendorDto>(vendor),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.Vendor),
            HttpStatusCode.Created
        );
    }

    private async Task InvalidateCacheAsync(Guid vendorId, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync($"{CacheKeys.Vendors}:{vendorId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorsPaged}:", cancellationToken);
    }
}
