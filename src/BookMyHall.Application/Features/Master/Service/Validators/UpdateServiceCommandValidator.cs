using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateServiceCommandValidator: AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty();

        RuleFor(x => x.ServiceName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ServiceIcon)
            .NotEmpty()
            .MaximumLength(500);
    }
}