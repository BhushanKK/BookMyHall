using FluentValidation;
namespace BookMyHall.Application.Features.Master;

public sealed class CreateCancellationPolicyCommandValidator
    : AbstractValidator<CreateCancellationPolicyCommand>
{
    public CreateCancellationPolicyCommandValidator()
    {
        RuleFor(x => x.PolicyName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.RefundPercentage)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.CancellationBeforeHours)
            .GreaterThanOrEqualTo(0);
    }
}