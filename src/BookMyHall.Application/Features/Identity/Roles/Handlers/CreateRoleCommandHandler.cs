using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Persistence.Exceptions;

namespace BookMyHall.Application.Features.Identity;

public sealed class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateRoleCommand> validator)
    : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
{
    public async Task<ApiResponse<RoleDto>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            return ApiResponse<RoleDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var role = mapper.Map<Role>(request);

        role.CreatedDate = DateTimeOffset.UtcNow;
        role.CreatedBy = Guid.Empty; // TODO: Replace with ICurrentUserService

        try
        {
            await roleRepository.AddAsync(role, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateRecordException)
        {
            return ApiResponse<RoleDto>.FailureResponse
            (
                string.Format(ApiMessages.RecordAlreadyExists, Entities.Role),
                HttpStatusCode.Conflict
            );
        }

        return ApiResponse<RoleDto>.SuccessResponse
        (
            mapper.Map<RoleDto>(role),
            string.Format(ApiMessages.RecordCreated, Entities.Role),
            HttpStatusCode.Created
        );
    }
}