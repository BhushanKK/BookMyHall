using FluentValidation;

namespace BookMyHall.Application.Features.Master;

public sealed class UpdateCityCommandValidator
    : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator()
    {
        RuleFor(x => x.CityId)
            .NotEmpty();

        RuleFor(x => x.DistrictId)
            .NotEmpty();

        RuleFor(x => x.CityName)
            .NotEmpty()
            .MaximumLength(100);
    }
}