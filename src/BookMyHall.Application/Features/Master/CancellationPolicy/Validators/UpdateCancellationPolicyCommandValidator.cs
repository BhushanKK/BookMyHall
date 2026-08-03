using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class UpdateCancellationPolicyCommandValidator: AbstractValidator<UpdateCancellationPolicyCommand>
{
    public UpdateCancellationPolicyCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.CancellationPolicyId)
            .Required(localizer, EntityKeys.CancellationPolicyId);

        RuleFor(x => x.PolicyName)
            .Required(localizer, EntityKeys.PolicyName)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .Required(localizer, EntityKeys.Description)
            .MaximumLength(500);

        RuleFor(x => x.RefundPercentage)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.CancellationBeforeHours)
            .GreaterThanOrEqualTo(0);
    }
}