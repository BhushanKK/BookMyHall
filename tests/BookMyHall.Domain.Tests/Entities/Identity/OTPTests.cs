using FluentAssertions;
using BookMyHall.Domain.Identity;

namespace BookMyHall.Domain.Tests.Entities.Identity;

public sealed class OTPTests
{
    [Fact]
    public void OTP_Should_Assign_OTPId()
    {
        var otp = new OTP();
        var id = Guid.NewGuid();

        otp.OTPId = id;

        otp.OTPId.Should().Be(id);
    }

    [Fact]
    public void OTP_Should_Assign_UserId()
    {
        var otp = new OTP();
        var userId = Guid.NewGuid();

        otp.UserId = userId;

        otp.UserId.Should().Be(userId);
    }

    [Fact]
    public void OTP_Should_Assign_MobileNumber()
    {
        var otp = new OTP();

        otp.MobileNumber = "9876543210";

        otp.MobileNumber.Should().Be("9876543210");
    }

    [Fact]
    public void OTP_Should_Assign_EmailAddress()
    {
        var otp = new OTP();

        otp.EmailAddress = "user@example.com";

        otp.EmailAddress.Should().Be("user@example.com");
    }

    [Fact]
    public void OTP_Should_Assign_OTPCode()
    {
        var otp = new OTP();

        otp.OTPCode = "123456";

        otp.OTPCode.Should().Be("123456");
    }

    [Fact]
    public void OTP_Should_Assign_OTPType()
    {
        var otp = new OTP();

        otp.OTPType = "MobileVerification";

        otp.OTPType.Should().Be("MobileVerification");
    }

    [Fact]
    public void OTP_Should_Assign_ExpiresAt()
    {
        var otp = new OTP();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(10);

        otp.ExpiresAt = expiresAt;

        otp.ExpiresAt.Should().Be(expiresAt);
    }

    [Fact]
    public void OTP_Should_Assign_VerifiedAt()
    {
        var otp = new OTP();
        var verifiedAt = DateTimeOffset.UtcNow;

        otp.VerifiedAt = verifiedAt;

        otp.VerifiedAt.Should().Be(verifiedAt);
    }

    [Fact]
    public void OTP_Should_Assign_All_Properties()
    {
        var otpId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
        var verifiedAt = DateTimeOffset.UtcNow;

        var otp = new OTP
        {
            OTPId = otpId,
            UserId = userId,
            MobileNumber = "9876543210",
            EmailAddress = "user@example.com",
            OTPCode = "123456",
            OTPType = "MobileVerification",
            ExpiresAt = expiresAt,
            VerifiedAt = verifiedAt
        };

        otp.OTPId.Should().Be(otpId);
        otp.UserId.Should().Be(userId);
        otp.MobileNumber.Should().Be("9876543210");
        otp.EmailAddress.Should().Be("user@example.com");
        otp.OTPCode.Should().Be("123456");
        otp.OTPType.Should().Be("MobileVerification");
        otp.ExpiresAt.Should().Be(expiresAt);
        otp.VerifiedAt.Should().Be(verifiedAt);
    }

    [Fact]
    public void OTP_Should_Have_Default_Values()
    {
        var otp = new OTP();

        otp.OTPId.Should().Be(Guid.Empty);
        otp.UserId.Should().Be(Guid.Empty);
        otp.MobileNumber.Should().BeEmpty();
        otp.EmailAddress.Should().BeEmpty();
        otp.OTPCode.Should().BeEmpty();
        otp.OTPType.Should().BeEmpty();
        otp.ExpiresAt.Should().Be(default);
        otp.VerifiedAt.Should().Be(default);
    }
}