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

public sealed class CreateCityCommandHandler(
    ICityRepository cityRepository,
    IUnitOfWork unitOfWork,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<CreateCityCommand, ApiResponse<Guid>>
{
    public async Task<ApiResponse<Guid>> Handle(CreateCityCommand request,CancellationToken cancellationToken)
    {
        var existingCity = await cityRepository.GetByCityNameAsync(request.CityName,cancellationToken);

        if (existingCity is not null)
        {
            return ApiResponse<Guid>.FailureResponse(messageHelper.AlreadyExists(EntityKeys.City),HttpStatusCode.BadRequest);
        }

        var city = mapper.Map<City>(request);
        city.CityId = Guid.NewGuid();
        city.IsActive = true;
        await cityRepository.AddAsync(city, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<Guid>.SuccessResponse(city.CityId,
            messageHelper.AddedEntity( ResourceNames.Entities,EntityKeys.City),HttpStatusCode.Created);
    }
}