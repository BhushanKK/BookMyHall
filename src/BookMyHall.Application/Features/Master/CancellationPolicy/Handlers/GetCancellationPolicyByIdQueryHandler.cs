using System.Net;
using AutoMapper;
using MediatR;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Application.Features.Master;

public sealed class GetCancellationPolicyByIdQueryHandler(
    ICancellationPolicyRepository cancellationPolicyRepository,
    IMessageHelper messageHelper,
    IMapper mapper)
    : IRequestHandler<GetCancellationPolicyByIdQuery, ApiResponse<CancellationPolicyDto>>
{
    public async Task<ApiResponse<CancellationPolicyDto>> Handle(
        GetCancellationPolicyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var policy = await cancellationPolicyRepository.GetByIdAsync(
            request.CancellationPolicyId,
            cancellationToken);

        if (policy is null)
        {
            return ApiResponse<CancellationPolicyDto>.FailureResponse(
                messageHelper.NotFound(EntityKeys.CancellationPolicy),
                HttpStatusCode.NotFound);
        }

        return ApiResponse<CancellationPolicyDto>.SuccessResponse(
            mapper.Map<CancellationPolicyDto>(policy),
            messageHelper.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.CancellationPolicy),
            HttpStatusCode.OK);
    }
}