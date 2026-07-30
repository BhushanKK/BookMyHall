using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Shared.Common;

public sealed class MessageHelper(
    ILocalizationService localizer)
    : IMessageHelper
{
    public string Entity(string resourceName, string key)
        => localizer.Get(resourceName, key);

    public string Added(string entity)
        => localizer.Get(ResourceNames.ApiMessageResponse, "Added", entity);

    public string Updated(string entity)
        => localizer.Get(ResourceNames.ApiMessageResponse, "Updated", entity);

    public string Deleted(string entity)
        => localizer.Get(ResourceNames.ApiMessageResponse, "Deleted", entity);

    public string Retrieved(string entity)
        => localizer.Get(ResourceNames.ApiMessageResponse, "Retrieved", entity);

    public string AlreadyExists(string entity)
        => localizer.Get(ResourceNames.ApiMessageResponse, "AlreadyExists", entity);

    public string NotFound(string entity)
        => localizer.Get(ResourceNames.ApiMessageResponse, "NotFound", entity);

    public string InvalidId(string entity)
        => localizer.Get(ResourceNames.ApiMessageResponse, "InvalidId", entity);

    public string AddedEntity(string resourceName, string key)
        => Added(Entity(resourceName, key));

    public string UpdatedEntity(string resourceName, string key)
        => Updated(Entity(resourceName, key));

    public string DeletedEntity(string resourceName, string key)
        => Deleted(Entity(resourceName, key));

    public string RetrievedEntity(string resourceName, string key)
        => Retrieved(Entity(resourceName, key));

    public string AlreadyExistsEntity(string resourceName, string key)
        => AlreadyExists(Entity(resourceName, key));

    public string NotFoundEntity(string resourceName, string key)
        => NotFound(Entity(resourceName, key));

    public string InvalidIdEntity(string resourceName, string key)
        => InvalidId(Entity(resourceName, key));
}