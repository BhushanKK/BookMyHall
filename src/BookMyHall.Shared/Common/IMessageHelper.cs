namespace BookMyHall.Shared.Common;

public interface IMessageHelper
{
    string Entity(string resourceName, string key);

    string Added(string entity);
    string Updated(string entity);
    string Deleted(string entity);
    string Retrieved(string entity);

    string AlreadyExists(string entity);
    string NotFound(string entity);
    string InvalidId(string entity);

    string AddedEntity(string resourceName, string key);
    string UpdatedEntity(string resourceName, string key);
    string DeletedEntity(string resourceName, string key);
    string RetrievedEntity(string resourceName, string key);
    string AlreadyExistsEntity(string resourceName, string key);
    string NotFoundEntity(string resourceName, string key);
    string InvalidIdEntity(string resourceName, string key);
    string LoginSuccessful();
    string LogoutSuccessful();
    string InvalidCredentials();
    string InvalidRefreshToken();
    string RefreshTokenExpired();
    string AccessDenied();
    string UserInactive();
    string PasswordChangedSuccessfully();
    string PasswordResetSuccessfully();
    string PasswordMismatch();
    string PasswordAlreadyUsed();
    string Unauthorized();
    string Forbidden();
    string UserLocked();
    string OtpSentSuccessfully();
    string InvalidOtp();
    string OtpExpired();
    string EmailVerifiedSuccessfully();
    string MobileVerifiedSuccessfully();
}