using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateServiceCommandValidator: AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.ServiceName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ServiceIcon)
            .NotEmpty()
            .MaximumLength(500);
    }
}