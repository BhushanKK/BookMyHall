using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class CreatePaymentModeCommandValidator: AbstractValidator<CreatePaymentModeCommand>
{
    public CreatePaymentModeCommandValidator()
    {
        RuleFor(x => x.PaymentModeName)
            .NotEmpty()
            .MaximumLength(100);
    }
}