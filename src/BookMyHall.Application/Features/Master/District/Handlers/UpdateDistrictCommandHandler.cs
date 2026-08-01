using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateDistrictCommandHandler(
    IDistrictRepository districtRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<UpdateDistrictCommand, ApiResponse<DistrictDto>>
{
    public async Task<ApiResponse<DistrictDto>> Handle(
        UpdateDistrictCommand request,
        CancellationToken cancellationToken)
    {
        var district = await districtRepository.GetByIdAsync(
            request.DistrictId,
            cancellationToken);

        if (district is null)
        {
            return ApiResponse<DistrictDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.District),
                HttpStatusCode.NotFound);
        }

        var existingDistrict = await districtRepository.GetByDistrictNameAsync(
            request.DistrictName,
            cancellationToken);

        if (existingDistrict is not null &&
            existingDistrict.DistrictId != request.DistrictId)
        {
            return ApiResponse<DistrictDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.District),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, district);

        await districtRepository.UpdateAsync(district, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var districtDto = mapper.Map<DistrictDto>(district);

        return ApiResponse<DistrictDto>.SuccessResponse(
            districtDto,
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.District),
            HttpStatusCode.OK);
    }
}