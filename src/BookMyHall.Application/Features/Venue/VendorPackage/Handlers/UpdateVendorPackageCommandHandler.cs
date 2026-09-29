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

public sealed class UpdateVendorPackageCommandHandler(IVendorPackageRepository repository,
    IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateVendorPackageCommand> validator,
    IMessageHelper messageHelper, ICacheService cacheService)
    : IRequestHandler<UpdateVendorPackageCommand, ApiResponse<VendorPackageDto>>
{
    public async Task<ApiResponse<VendorPackageDto>> Handle(UpdateVendorPackageCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<VendorPackageDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var package = await repository.GetByIdAsync(request.VendorPackageId, cancellationToken);
        if (package is null)
        {
            return ApiResponse<VendorPackageDto>.FailureResponse(messageHelper.NotFound(EntityKeys.VendorPackage), HttpStatusCode.NotFound);
        }

        mapper.Map(request, package);
        await repository.UpdateAsync(package, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveAsync($"{CacheKeys.VendorPackages}:{package.VendorPackageId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.VendorPackagesPaged}:", cancellationToken);

        return ApiResponse<VendorPackageDto>.SuccessResponse(mapper.Map<VendorPackageDto>(package),
            messageHelper.UpdatedEntity(ResourceNames.Entities, EntityKeys.VendorPackage), HttpStatusCode.OK);
    }
}
