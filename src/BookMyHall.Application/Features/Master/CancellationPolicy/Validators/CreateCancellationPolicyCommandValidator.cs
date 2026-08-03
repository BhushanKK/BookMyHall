using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class CreateCancellationPolicyCommandValidator
    : AbstractValidator<CreateCancellationPolicyCommand>
{
    public CreateCancellationPolicyCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.PolicyName)
            .Required(localizer, EntityKeys.PolicyName)
            .MaximumLengthLocalized(localizer, EntityKeys.PolicyName, 100);

        RuleFor(x => x.Description)
            .Required(localizer, EntityKeys.Description)
            .MaximumLengthLocalized(localizer, EntityKeys.Description, 500);

        RuleFor(x => x.RefundPercentage)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.CancellationBeforeHours)
            .GreaterThanOrEqualTo(0);
    }
}