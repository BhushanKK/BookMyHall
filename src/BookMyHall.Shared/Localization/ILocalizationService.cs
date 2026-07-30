namespace BookMyHall.Shared.Localization;

public interface ILocalizationService
{
    string Get(string resourceName, string key);

    string Get(string resourceName, string key, params object[] arguments);
}