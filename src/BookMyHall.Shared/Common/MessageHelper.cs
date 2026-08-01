using BookMyHall.Shared.Constants;
using BookMyHall.Shared.Localization;

namespace BookMyHall.Shared.Common;

public sealed class MessageHelper(ILocalizationService localizer)
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

    public string LoginSuccessful()
        => localizer.Get(ResourceNames.ApiMessageResponse, "LoginSuccessful");

    public string LogoutSuccessful()
        => localizer.Get(ResourceNames.ApiMessageResponse, "LogoutSuccessful");

    public string InvalidCredentials()
        => localizer.Get(ResourceNames.ApiMessageResponse, "InvalidCredentials");

    public string InvalidRefreshToken()
        => localizer.Get(ResourceNames.ApiMessageResponse, "InvalidRefreshToken");

    public string RefreshTokenExpired()
        => localizer.Get(ResourceNames.ApiMessageResponse, "RefreshTokenExpired");

    public string AccessDenied()
        => localizer.Get(ResourceNames.ApiMessageResponse, "AccessDenied");

    public string UserInactive()
        => localizer.Get(ResourceNames.ApiMessageResponse, "UserInactive");

    public string PasswordChangedSuccessfully()
        => localizer.Get(ResourceNames.ApiMessageResponse, "PasswordChangedSuccessfully");

    public string PasswordResetSuccessfully()
        => localizer.Get(ResourceNames.ApiMessageResponse, "PasswordResetSuccessfully");

    public string PasswordMismatch()
    => localizer.Get(ResourceNames.ApiMessageResponse, "PasswordMismatch");

    public string PasswordAlreadyUsed()
        => localizer.Get(ResourceNames.ApiMessageResponse, "PasswordAlreadyUsed");

    public string Unauthorized()
        => localizer.Get(ResourceNames.ApiMessageResponse, "Unauthorized");

    public string Forbidden()
        => localizer.Get(ResourceNames.ApiMessageResponse, "Forbidden");

    public string UserLocked()
    => localizer.Get(ResourceNames.ApiMessageResponse, "UserLocked");

    public string OtpSentSuccessfully()
        => localizer.Get(ResourceNames.ApiMessageResponse, "OtpSentSuccessfully");

    public string InvalidOtp()
        => localizer.Get(ResourceNames.ApiMessageResponse, "InvalidOtp");

    public string OtpExpired()
        => localizer.Get(ResourceNames.ApiMessageResponse, "OtpExpired");

    public string EmailVerifiedSuccessfully()
        => localizer.Get(ResourceNames.ApiMessageResponse, "EmailVerifiedSuccessfully");

    public string MobileVerifiedSuccessfully()
        => localizer.Get(ResourceNames.ApiMessageResponse, "MobileVerifiedSuccessfully");
}