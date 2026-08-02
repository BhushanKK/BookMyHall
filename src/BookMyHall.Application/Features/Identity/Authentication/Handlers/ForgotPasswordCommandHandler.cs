using MediatR;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Application.Abstractions.Persistence;
using BookMyHall.Application.Abstractions.Persistence.Identity;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Application.Features.Authentication.Commands.ForgotPassword;

namespace BookMyHall.Application.Features.Identity.Authentication.Handlers;

public sealed class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    ITokenGenerator tokenGenerator,
    ITokenHasher tokenHasher,
    IUnitOfWork unitOfWork)
        : IRequestHandler<ForgotPasswordCommand, ApiResponse<ForgotPasswordResponse>>
{
    private const int PasswordResetTokenExpiryInMinutes = 30;
    public async Task<ApiResponse<ForgotPasswordResponse>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var response = CreateSuccessResponse();
        var user = await userRepository.GetByEmailAddressAsync(request.Email, cancellationToken);

        // Prevent user enumeration.
        if (user is null)
            return response;

        // Remove existing password reset tokens.
        await passwordResetTokenRepository.DeleteByUserIdAsync(user.UserId, cancellationToken);

        // Generate secure token.
        var resetToken = tokenGenerator.GeneratePasswordResetToken();

        // Hash before storing.
        var tokenHash = tokenHasher.Hash(resetToken);

        var passwordResetToken = PasswordResetToken.Create
        (
            userId: user.UserId,
            tokenHash: tokenHash,
            expiresAt: DateTimeOffset.UtcNow.AddMinutes(PasswordResetTokenExpiryInMinutes)
        );

        await passwordResetTokenRepository.AddAsync(passwordResetToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        response.Data?.ResetToken = resetToken;
        return response;
    }

    private static ApiResponse<ForgotPasswordResponse> CreateSuccessResponse()
    {
        return ApiResponse<ForgotPasswordResponse>.SuccessResponse
        (
            new ForgotPasswordResponse
            {
                Message = "If an account exists for the provided email address, a password reset token has been generated."
            }
        );
    }
}
