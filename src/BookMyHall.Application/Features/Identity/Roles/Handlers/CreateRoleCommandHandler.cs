using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateRoleCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
{
    public async Task<ApiResponse<RoleDto>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(
                " | ",
                validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<RoleDto>.FailureResponse(
                message,
                HttpStatusCode.BadRequest);
        }

        var role = mapper.Map<Role>(request);

        try
        {
            await roleRepository.AddAsync(role, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<RoleDto>.FailureResponse
            (
                messageHelper.AlreadyExistsEntity(ResourceNames.Entities,EntityKeys.Role),
                HttpStatusCode.Conflict
            );
        }

        return ApiResponse<RoleDto>.SuccessResponse
        (
            mapper.Map<RoleDto>(role),
            messageHelper.AddedEntity(ResourceNames.Entities,EntityKeys.Role),
            HttpStatusCode.Created
        );
    }
}