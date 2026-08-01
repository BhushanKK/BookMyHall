using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateDistrictCommandValidator
    : AbstractValidator<CreateDistrictCommand>
{
    public CreateDistrictCommandValidator()
    {
        RuleFor(x => x.StateId)
            .NotEmpty();

        RuleFor(x => x.DistrictName)
            .NotEmpty()
            .MaximumLength(100);
    }
}