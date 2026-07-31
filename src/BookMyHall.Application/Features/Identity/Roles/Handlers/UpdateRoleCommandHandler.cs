using MediatR;
using System.Net;
using AutoMapper;
using FluentValidation;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateRoleCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<UpdateRoleCommand, ApiResponse<RoleDto>>
{
    public async Task<ApiResponse<RoleDto>> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(e => e.ErrorMessage));
            return ApiResponse<RoleDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return ApiResponse<RoleDto>.FailureResponse
            (
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.Role),
                HttpStatusCode.NotFound
            );
        }

        mapper.Map(request, role);

        try
        {
            await roleRepository.UpdateAsync(role, cancellationToken);
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
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.Role),
            HttpStatusCode.OK
        );
    }
}