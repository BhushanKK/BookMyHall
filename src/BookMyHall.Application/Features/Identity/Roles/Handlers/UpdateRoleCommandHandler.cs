using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Constants;
using BookMyHall.Persistence.Exceptions;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateRoleCommand> validator)
    : IRequestHandler<UpdateRoleCommand, ApiResponse<RoleDto>>
{
    public async Task<ApiResponse<RoleDto>> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(e => e.ErrorMessage));
            return ApiResponse<RoleDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var role = await roleRepository.GetByIdAsync(request.RoleId,cancellationToken);

        if (role is null)
        {
            return ApiResponse<RoleDto>.FailureResponse
            (
                string.Format(ApiMessages.RecordNotFound, Entities.Role),
                HttpStatusCode.NotFound
            );
        }

        mapper.Map(request, role);

        role.UpdatedDate = DateTimeOffset.UtcNow;
        role.UpdatedBy = Guid.Empty; // TODO: Replace with ICurrentUserService

        try
        {
            await roleRepository.UpdateAsync(role, cancellationToken);
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
            string.Format(ApiMessages.RecordUpdated, Entities.Role),
            HttpStatusCode.OK
        );
    }
}