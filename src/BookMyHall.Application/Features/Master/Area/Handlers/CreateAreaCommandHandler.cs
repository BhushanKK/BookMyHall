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

public sealed class CreateAreaCommandHandler(
    IAreaRepository areaRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreateAreaCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(
        CreateAreaCommand request,
        CancellationToken cancellationToken)
    {
        var existingArea = await areaRepository.GetByAreaNameAsync(
            request.AreaName,
            cancellationToken);

        if (existingArea is not null)
        {
            return ApiResponse<Guid>.FailureResponse(
                messageHelper.AlreadyExists(EntityKeys.Area),
                HttpStatusCode.BadRequest);
        }

        var area = mapper.Map<Area>(request);

        area.AreaId = Guid.NewGuid();
        area.IsActive = true;

        await areaRepository.AddAsync(area, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(
            area.AreaId,
            messageHelper.AddedEntity(
                ResourceNames.Entities,
                EntityKeys.Area),
            HttpStatusCode.Created);
    }
}