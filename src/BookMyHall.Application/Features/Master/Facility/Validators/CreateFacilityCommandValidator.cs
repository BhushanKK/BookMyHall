using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateFacilityCommandValidator: AbstractValidator<CreateFacilityCommand>
{
    public CreateFacilityCommandValidator()
    {
        RuleFor(x => x.FacilityName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.FacilityIcon)
            .NotEmpty()
            .MaximumLength(500);
    }
}