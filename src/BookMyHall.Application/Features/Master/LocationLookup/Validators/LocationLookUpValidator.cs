using FluentValidation;
using BookMyHall.Application.Master.LocationLookup.Queries;
namespace BookMyHall.Application.Master.LocationLookup.Validators;
public sealed class GetStatesQueryValidator: AbstractValidator<GetStatesQuery>
{
    public GetStatesQueryValidator()
    {
        RuleFor(x => x.CountryId)
            .NotEmpty()
            .WithMessage("Country is required.");
    }
}


public sealed class GetDistrictsQueryValidator: AbstractValidator<GetDistrictsQuery>
{
    public GetDistrictsQueryValidator()
    {
        RuleFor(x => x.StateId)
            .NotEmpty()
            .WithMessage("State is required.");
    }
}


public sealed class GetCitiesQueryValidator: AbstractValidator<GetCitiesQuery>
{
    public GetCitiesQueryValidator()
    {
        RuleFor(x => x.DistrictId)
            .NotEmpty()
            .WithMessage("District is required.");
    }
}


public sealed class GetAreasQueryValidator: AbstractValidator<GetAreasQuery>
{
    public GetAreasQueryValidator()
    {
        RuleFor(x => x.CityId)
            .NotEmpty()
            .WithMessage("City is required.");
    }
}