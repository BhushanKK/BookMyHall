using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using BookMyHall.Application.Abstractions.Authentication;
using BookMyHall.Infrastructure.Authentication;
using BookMyHall.Domain.Dtos;

namespace BookMyHall.Infrastructure.Tests.Authentication;

public sealed class JwtTokenServiceTests
{
    private readonly JwtOptions _options;
    private readonly JwtTokenService _service;

    public JwtTokenServiceTests()
    {
        _options = new JwtOptions
        {
            Issuer = "BookMyHall",
            Audience = "BookMyHallUsers",
            SecretKey = "ThisIsASuperSecretKeyWithMinimum32Characters!",
            AccessTokenExpiryMinutes = 60
        };

        _service = new JwtTokenService(Microsoft.Extensions.Options.Options.Create(_options));
    }

    private static JwtUser CreateUser()
    {
        return new JwtUser
        {
            UserId = Guid.NewGuid(),
            FullName = "Bhushan Kachave",
            EmailAddress = "bhushankachave@bookmyhall.com",
            MobileNumber = "9876543210",
            Roles =
            [
                new JwtRole
            {
                RoleId = Guid.NewGuid(),
                RoleName = "Admin"
            },
            new JwtRole
            {
                RoleId = Guid.NewGuid(),
                RoleName = "HallOwner"
            }
        ]
    };
}

    [Fact]
    public void GenerateToken_Should_Return_Valid_JwtToken()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var result = _service.GenerateToken(user);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.ExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Fact]
    public void GenerateToken_Should_Contain_Correct_Claims()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var result = _service.GenerateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.AccessToken);

        // Assert

        jwt.Claims.Should().Contain(x =>
            x.Type == JwtRegisteredClaimNames.Sub &&
            x.Value == user.UserId.ToString());

        jwt.Claims.Should().Contain(x =>
            x.Type == JwtRegisteredClaimNames.UniqueName &&
            x.Value == user.FullName);

        jwt.Claims.Should().Contain(x =>
            x.Type == JwtRegisteredClaimNames.Email &&
            x.Value == user.EmailAddress);

        jwt.Claims.Should().Contain(x =>
            x.Type == ClaimTypes.MobilePhone &&
            x.Value == user.MobileNumber);

        jwt.Claims.Should().Contain(x =>
            x.Type == ClaimTypes.Role &&
            x.Value == "Admin");

        jwt.Claims.Should().Contain(x =>
            x.Type == ClaimTypes.Role &&
            x.Value == "HallOwner");
    }

    [Fact]
    public void GenerateToken_Should_Set_Issuer()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var result = _service.GenerateToken(user);

        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(result.AccessToken);

        // Assert
        token.Issuer.Should().Be(_options.Issuer);
    }

    [Fact]
    public void GenerateToken_Should_Set_Audience()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var result = _service.GenerateToken(user);

        var token = new JwtSecurityTokenHandler()
            .ReadJwtToken(result.AccessToken);

        // Assert
        token.Audiences.Should().Contain(_options.Audience);
    }

    [Fact]
    public void GenerateToken_Should_Throw_When_User_Is_Null()
    {
        // Act
        Action action = () => _service.GenerateToken(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenerateRefreshToken_Should_Return_NonEmpty_String()
    {
        // Act
        var token = _service.GenerateRefreshToken();

        // Assert
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateRefreshToken_Should_Return_Different_Value_Each_Time()
    {
        // Act
        var token1 = _service.GenerateRefreshToken();
        var token2 = _service.GenerateRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GenerateRefreshToken_Should_Be_Base64()
    {
        // Act
        var token = _service.GenerateRefreshToken();

        // Assert
        var action = () => Convert.FromBase64String(token);

        action.Should().NotThrow();
    }

    [Fact]
    public void GenerateToken_Should_Set_Expiry_Based_On_Configuration()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var result = _service.GenerateToken(user);

        // Assert
        result.ExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow.AddMinutes(59));
        result.ExpiresAt.Should().BeBefore(DateTimeOffset.UtcNow.AddMinutes(61));
    }
}