using System.Net;
using FluentValidation;
using MediatR;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class DeleteHallCategoryCommandHandler(
    IHallCategoryRepository hallCategoryRepository,
    IUnitOfWork unitOfWork,
    IValidator<DeleteHallCategoryCommand> validator,
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteHallCategoryCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteHallCategoryCommand request,CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync( request,cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));

            return ApiResponse<bool>.FailureResponse(message,HttpStatusCode.BadRequest);
        }

        var category = await hallCategoryRepository.GetByIdAsync(request.HallCategoryId,cancellationToken);

        if (category is null)
        {
            return ApiResponse<bool>.FailureResponse(
                messageHelper.NotFoundEntity(ResourceNames.Entities,EntityKeys.HallCategory),HttpStatusCode.NotFound);
        }

        category.IsActive = false;
        await hallCategoryRepository.UpdateAsync(category,cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.SuccessResponse( true,
            messageHelper.DeletedEntity(ResourceNames.Entities,EntityKeys.HallCategory),HttpStatusCode.OK);
    }
}