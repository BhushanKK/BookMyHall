using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCancellationPolicyByIdQueryHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCancellationPolicyByIdQuery, ApiResponse<CancellationPolicy>>
{
    public async Task<ApiResponse<CancellationPolicy>> Handle(GetCancellationPolicyByIdQuery request,CancellationToken cancellationToken)
    {
        var policy = await cancellationPolicyRepository.GetByIdAsync(request.CancellationPolicyId,cancellationToken);
        if (policy is null)
        {
            return ApiResponse<CancellationPolicy>.FailureResponse(
                messageHelper.NotFound(EntityKeys.CancellationPolicy),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<CancellationPolicy>.SuccessResponse(
            mapper.Map<CancellationPolicy>(policy),
            messageHelper.RetrievedEntity(ResourceNames.Entities,EntityKeys.CancellationPolicy),HttpStatusCode.OK);
    }
}