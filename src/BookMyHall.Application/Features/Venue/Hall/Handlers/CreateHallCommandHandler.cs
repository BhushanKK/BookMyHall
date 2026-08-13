using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
namespace BookMyHall.Application.Features.Venue;

public sealed class CreateHallCommandHandler(
    IHallRepository hallRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateHallCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<CreateHallCommand, ApiResponse<HallDto>>
{
    public async Task<ApiResponse<HallDto>> Handle(CreateHallCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<HallDto>.FailureResponse(message, HttpStatusCode.BadRequest);
        }

        var hall = mapper.Map<Hall>(request);

        try
        {
            await hallRepository.AddAsync(hall, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<HallDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.Hall),
                HttpStatusCode.Conflict
            );
        }

        return ApiResponse<HallDto>.SuccessResponse
        (
            mapper.Map<HallDto>(hall),
            messageHelper.AddedEntity(ResourceNames.Entities, EntityKeys.Hall),
            HttpStatusCode.Created
        );
    }
}