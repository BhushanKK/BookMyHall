namespace BookMyHall.Infrastructure.Configuration;

public static class RabbitMqKeys
{
    public const string UserRegistrationQueueName = "identity.user.registration";
    public const string UserRegistrationRoutingKey = "identity.user.registered";
    public const string PasswordChangedQueueName = "identity.password.changed";
    public const string PasswordChangedRoutingKey = "identity.password.changed";
    public const string PasswordResetQueueName = "identity.password.reset";
    public const string PasswordResetRoutingKey = "identity.password.reset.requested";
    public const string PasswordResetSuccessQueueName = "identity.password.reset.success";
    public const string PasswordResetSuccessRoutingKey = "identity.password.reset.success";
}