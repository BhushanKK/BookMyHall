using FluentValidation;

namespace BookMyHall.Application.Features.Venue.VendorServiceAreas;
public sealed class CreateVendorServiceAreaCommandValidator: AbstractValidator<CreateVendorServiceAreaCommand>
{
    public CreateVendorServiceAreaCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .NotEmpty()
            .WithMessage("Vendor is required.");

        RuleFor(x => x.ServiceRadiusKm)
            .GreaterThan(0)
            .When(x => x.ServiceRadiusKm.HasValue)
            .WithMessage("Service radius must be greater than zero.");
    }
}

public sealed class UpdateVendorServiceAreaCommandValidator: AbstractValidator<UpdateVendorServiceAreaCommand>
{
    public UpdateVendorServiceAreaCommandValidator()
    {
        RuleFor(x => x.VendorServiceAreaId)
            .NotEmpty()
            .WithMessage("Vendor service area is required.");

        RuleFor(x => x.VendorId)
            .NotEmpty()
            .WithMessage("Vendor is required.");

        RuleFor(x => x.ServiceRadiusKm)
            .GreaterThan(0)
            .When(x => x.ServiceRadiusKm.HasValue)
            .WithMessage("Service radius must be greater than zero.");
    }
}

public sealed class GetVendorServiceAreasQueryValidator: AbstractValidator<GetVendorServiceAreasQuery>
{
    public GetVendorServiceAreasQueryValidator()
    {
        RuleFor(x => x.Pagination.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.Pagination.PageSize)
            .GreaterThan(0);
    }
}