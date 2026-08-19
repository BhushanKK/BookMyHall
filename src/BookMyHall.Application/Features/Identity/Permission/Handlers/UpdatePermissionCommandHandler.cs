using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Identity;

public sealed class UpdatePermissionCommandHandler(
    IPermissionRepository permissionRepository,
    IUnitOfWork unitOfWork,IMapper mapper,
    IValidator<UpdatePermissionCommand> validator,
    IMessageHelper messageHelper): IRequestHandler<UpdatePermissionCommand, ApiResponse<PermissionDto>>
{
    public async Task<ApiResponse<PermissionDto>> Handle(UpdatePermissionCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));
            return ApiResponse<PermissionDto>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var permission = await permissionRepository.GetByIdAsync(request.PermissionId,cancellationToken);

        if (permission is null)
        {
            return ApiResponse<PermissionDto>.FailureResponse(messageHelper.NotFoundEntity(
                    ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.NotFound);
        }

        mapper.Map(request, permission);
        await permissionRepository.UpdateAsync(permission,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<PermissionDto>.SuccessResponse(mapper.Map<PermissionDto>(permission),
            messageHelper.UpdatedEntity(ResourceNames.Entities,EntityKeys.Permission),HttpStatusCode.OK);
    }
}