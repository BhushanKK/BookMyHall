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

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateDistrictCommandHandler(
    IDistrictRepository districtRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateDistrictCommand> validator,
    IMessageHelper messageHelper,ICacheService cacheService)
    : IRequestHandler<UpdateDistrictCommand, ApiResponse<DistrictDto>>
{
    public async Task<ApiResponse<DistrictDto>> Handle(UpdateDistrictCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<DistrictDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var district = await districtRepository.GetByIdAsync(request.DistrictId,cancellationToken);
        if (district is null)
        {
            return ApiResponse<DistrictDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.District),
                HttpStatusCode.NotFound);
        }

        var existingDistrict = await districtRepository.GetByDistrictNameAsync(request.DistrictName,cancellationToken);

        if (existingDistrict is not null && existingDistrict.DistrictId != request.DistrictId)
        {
            return ApiResponse<DistrictDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.District),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, district);
        await districtRepository.UpdateAsync(district,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveAsync($"{CacheKeys.Districts}:{request.DistrictId}", cancellationToken);
        await cacheService.RemoveByPrefixAsync($"{CacheKeys.DistrictsPaged}:", cancellationToken);
        return ApiResponse<DistrictDto>.SuccessResponse(
            mapper.Map<DistrictDto>(district),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.District), HttpStatusCode.OK);
    }
}