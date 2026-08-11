using FluentValidation;

using BookMyHall.Application.Common.Extensions;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Application.Validations;

public sealed class CreateDeviceCommandValidator : AbstractValidator<RegisterDeviceCommand>
{
    public CreateDeviceCommandValidator(ILocalizationService localizer)
    {
        RuleFor(x => x.UserId)
            .Required(localizer, EntityKeys.User);

        RuleFor(x => x.DeviceIdentifier)
            .Required(localizer, EntityKeys.DeviceIdentifier)
            .MaximumLengthLocalized(localizer, EntityKeys.DeviceIdentifier, 250);

        RuleFor(x => x.DeviceType)
            .Required(localizer, EntityKeys.DeviceType)
            .MaximumLengthLocalized(localizer, EntityKeys.DeviceType, 50);

        RuleFor(x => x.PushNotificationToken)
            .MaximumLengthLocalized(localizer, EntityKeys.PushNotificationToken, 500);

        RuleFor(x => x.DeviceName)
            .MaximumLengthLocalized(localizer, EntityKeys.DeviceName, 100);

        RuleFor(x => x.OperatingSystem)
            .MaximumLengthLocalized(localizer, EntityKeys.OperatingSystem, 100);

        RuleFor(x => x.Browser)
            .MaximumLengthLocalized(localizer, EntityKeys.Browser, 100);

        RuleFor(x => x.AppVersion)
            .MaximumLengthLocalized(localizer, EntityKeys.AppVersion, 50);

        RuleFor(x => x.LastIpAddress)
            .MaximumLengthLocalized(localizer, EntityKeys.LastIpAddress, 100);
    }
}