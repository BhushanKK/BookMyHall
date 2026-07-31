using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateAmenityValidator: AbstractValidator<CreateAmenityCommand>
{
    public CreateAmenityValidator()
    {
        RuleFor(x => x.AmenityName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.AmenityIcon)
            .MaximumLength(250);
    }
}