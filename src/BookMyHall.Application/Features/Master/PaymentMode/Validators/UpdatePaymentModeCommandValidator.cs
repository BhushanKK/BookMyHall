using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdatePaymentModeCommandValidator: AbstractValidator<UpdatePaymentModeCommand>
{
    public UpdatePaymentModeCommandValidator()
    {
        RuleFor(x => x.PaymentModeId)
            .NotEmpty();

        RuleFor(x => x.PaymentModeName)
            .NotEmpty()
            .MaximumLength(100);
    }
}