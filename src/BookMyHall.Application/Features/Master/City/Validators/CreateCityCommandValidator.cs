using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class CreateCityCommandValidator
    : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.CityName)
            .NotEmpty()
            .MaximumLength(100);
    }
}