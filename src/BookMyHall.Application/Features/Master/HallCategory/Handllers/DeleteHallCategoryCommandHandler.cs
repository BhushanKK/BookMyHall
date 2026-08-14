using System.Net;

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
    IMessageHelper messageHelper)
    : IRequestHandler<DeleteHallCategoryCommand,ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteHallCategoryCommand request,CancellationToken cancellationToken)
    {
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