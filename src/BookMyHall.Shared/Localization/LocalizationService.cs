using System.Globalization;
using System.Resources;
using BookMyHall.Shared.Constants;

namespace BookMyHall.Shared.Localization;

public sealed class LocalizationService : ILocalizationService
{
    private readonly Dictionary<string, ResourceManager> _resourceManagers = [];

    public LocalizationService()
    {
        _resourceManagers.Add(
            ResourceNames.ApiMessageResponse,
            new ResourceManager(
                "BookMyHall.Shared.Localization.ApiMessageResponse",
                typeof(ApiMessageResponse).Assembly));

        _resourceManagers.Add(
            ResourceNames.Entities,
            new ResourceManager(
                "BookMyHall.Shared.Localization.Entities",
                typeof(Entities).Assembly));

        _resourceManagers.Add(
            ResourceNames.ValidationMessages,
            new ResourceManager(
                "BookMyHall.Shared.Localization.ValidationMessages",
                typeof(ValidationMessages).Assembly));
    }

    public string Get(string resourceName, string key)
    {
        if (!_resourceManagers.TryGetValue(resourceName, out var manager))
            return key;

        return manager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }

    public string Get(string resourceName, string key, params object[] arguments)
    {
        var value = Get(resourceName, key);

        return string.Format(value, arguments);
    }
}