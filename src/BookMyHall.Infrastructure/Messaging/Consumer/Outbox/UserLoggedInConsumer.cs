using System.Text.Json;

using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Events.Identity;
using BookMyHall.Domain.Audit;
using BookMyHall.Domain.Common;
using BookMyHall.Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookMyHall.Infrastructure.Messaging.Consumers;

public sealed class UserLoggedInConsumer(
    IServiceScopeFactory scopeFactory,
    ILogger<UserLoggedInConsumer> logger)
{
    public async Task ConsumeAsync(
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        var loginEvent = JsonSerializer.Deserialize<UserLoggedInEvent>(
            body.Span);

        if (loginEvent is null)
            throw new InvalidOperationException(
                "Unable to deserialize UserLoggedInEvent.");

        using var scope = scopeFactory.CreateScope();

        var userLoginHistoryRepository =
            scope.ServiceProvider.GetRequiredService<IUserLoginHistoryRepository>();

        var loginHistory = new UserLoginHistory
        {
            UserLoginHistoryId = Guid.NewGuid(),
            UserId = loginEvent.UserId,
            SessionId = loginEvent.SessionId,
            LoginDate = loginEvent.LoginDate,
            LoginStatus = LoginStatuses.Success,
            LoginMethod = loginEvent.LoginMethod,
            IpAddress = loginEvent.IpAddress ?? "Unknown",
            UserAgent = loginEvent.UserAgent ?? "Unknown",
            Browser = loginEvent.Browser ?? "Unknown",
            OperatingSystem = loginEvent.OperatingSystem ?? "Unknown",
            DeviceType = loginEvent.DeviceType ?? "Unknown",
            LoginSource = loginEvent.LoginSource ?? "Unknown",
            IsMfaUsed = loginEvent.IsMfaUsed
        };

        await userLoginHistoryRepository.AddAsync(
            loginHistory,
            cancellationToken);

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "User login history created. UserId: {UserId}, SessionId: {SessionId}",
            loginEvent.UserId,
            loginEvent.SessionId);
    }
}