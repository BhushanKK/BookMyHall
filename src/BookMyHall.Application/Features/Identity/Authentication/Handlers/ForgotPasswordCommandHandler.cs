using MediatR;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Messaging;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Authentication.Commands.ForgotPassword;
using BookMyHall.Contracts.Common;
using BookMyHall.Contracts.Messaging;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Application.Features.Identity.Authentication.Handlers;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    ITokenGenerator tokenGenerator,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork,
    IMessagePublisher messagePublisher)
    : IRequestHandler<ForgotPasswordCommand, ApiResponse<ForgotPasswordResponse>>
{
    private const int PasswordResetTokenExpiryInMinutes = 30;

    public async Task<ApiResponse<ForgotPasswordResponse>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var response = CreateSuccessResponse();

        var user = await userRepository.GetByEmailAddressAsync
        (
            request.Email,
            cancellationToken
        );

        if (user is null)
        {
            return response;
        }

        await passwordResetTokenRepository.DeleteByUserIdAsync
        (
            user.UserId,
            cancellationToken
        );

        var resetToken = tokenGenerator.GeneratePasswordResetToken();

        var tokenHash = tokenHasher.Hash(resetToken);

        var passwordResetToken = PasswordResetToken.Create
        (
            userId: user.UserId,
            tokenHash: tokenHash,
            expiresAt: DateTimeOffset.UtcNow.AddMinutes(PasswordResetTokenExpiryInMinutes)
        );

        await passwordResetTokenRepository.AddAsync
        (
            passwordResetToken,
            cancellationToken
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var passwordResetMessage = new PasswordResetRequestedMessage
        (
            user.UserId,
            user.FullName,
            user.EmailAddress,
            resetToken
        );

        await messagePublisher.PublishAsync
        (
            passwordResetMessage,
            cancellationToken
        );

        return response;
    }

    private static ApiResponse<ForgotPasswordResponse> CreateSuccessResponse()
    {
        return ApiResponse<ForgotPasswordResponse>.SuccessResponse
        (
            new ForgotPasswordResponse
            {
                Message = "If an account exists for the provided email address, a password reset email has been sent."
            }
        );
    }
}