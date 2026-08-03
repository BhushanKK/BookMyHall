using FluentValidation;
using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Master;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;
namespace BookMyHall.Application.Validations;

public sealed class UpdateAmenityValidator: AbstractValidator<UpdateAmenityCommand>
{
    public UpdateAmenityValidator(ILocalizationService localizer)
    {
        RuleFor(x=>x.AmenityId)
            .Required(localizer, EntityKeys.AmenityId);
            

        RuleFor(x => x.AmenityName)
            .Required(localizer, EntityKeys.Amenity)
            .MaximumLengthLocalized(localizer, EntityKeys.Amenity, 100);

            RuleFor(x => x.AmenityIcon)
            .Required(localizer, EntityKeys.AmenityIcon)
            .MaximumLengthLocalized(localizer, EntityKeys.AmenityIcon, 250);            
    }
}