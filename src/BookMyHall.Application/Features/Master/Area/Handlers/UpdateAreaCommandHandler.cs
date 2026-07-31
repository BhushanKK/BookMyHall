using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
namespace BookMyHall.Application.Features.Master;

public sealed class UpdateAreaCommandHandler(
    IAreaRepository areaRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<UpdateAreaCommand, ApiResponse<AreaDto>>
{
    public async Task<ApiResponse<AreaDto>> Handle(
        UpdateAreaCommand request,
        CancellationToken cancellationToken)
    {
        var area = await areaRepository.GetByIdAsync(
            request.AreaId,
            cancellationToken);

        if (area is null)
        {
            return ApiResponse<AreaDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.Area),
                HttpStatusCode.NotFound);
        }

        var existingArea = await areaRepository.GetByAreaNameAsync(
            request.AreaName,
            cancellationToken);

        if (existingArea is not null &&
            existingArea.AreaId != request.AreaId)
        {
            return ApiResponse<AreaDto>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Area),
                HttpStatusCode.BadRequest);
        }

        mapper.Map(request, area);

        await areaRepository.UpdateAsync(area, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<AreaDto>.SuccessResponse(
            mapper.Map<AreaDto>(area),
            messageHelper.UpdatedEntity(
                ResourceNames.Entities,
                EntityKeys.Area),
            HttpStatusCode.OK);
    }
}