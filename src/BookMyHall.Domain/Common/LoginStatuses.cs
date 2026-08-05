namespace BookMyHall.Domain.Common;
public static class LoginStatuses
{
    public const string Success = "Success";
    public const string Failed = "Failed";
    public const string Locked = "Locked";
    public const string LoggedOut = "LoggedOut";
}

public static class LoginMethods
{
    public const string Password = "Password";
    public const string Google = "Google";
    public const string Microsoft = "Microsoft";
    public const string OTP = "OTP";
}