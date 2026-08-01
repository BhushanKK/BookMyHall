using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateDistrictCommandHandler(
    IDistrictRepository districtRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreateDistrictCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateDistrictCommand request,CancellationToken cancellationToken)
    {
        var existingDistrict = await districtRepository.GetByDistrictNameAsync(request.DistrictName,cancellationToken);

        if (existingDistrict is not null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.District),
                HttpStatusCode.BadRequest);
        }

        var district = mapper.Map<District>(request);
        district.DistrictId = Guid.NewGuid();
        district.IsActive = true;
        await districtRepository.AddAsync(district, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<Guid>.SuccessResponse(
            district.DistrictId,
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.District),HttpStatusCode.Created);
    }
}