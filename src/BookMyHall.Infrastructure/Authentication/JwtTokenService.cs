using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BookMyHall.Application.Abstractions.Authentication;

namespace BookMyHall.Infrastructure.Authentication;

public sealed class JwtTokenService(IOptions<JwtOptions> options)
    : IJwtTokenService
{
    private const string TokenVersionClaim = "token_version";

    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    private readonly JwtOptions _options = options.Value
        ?? throw new ArgumentNullException(nameof(options));

    public JwtTokenResult GenerateToken(JwtUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var claims = BuildClaims(user);

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(
            _options.AccessTokenExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: CreateSigningCredentials());

        return new JwtTokenResult
        {
            AccessToken = TokenHandler.WriteToken(token),
            ExpiresAt = expiresAt
        };
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    private static List<Claim> BuildClaims(JwtUser user)
    {
        var claims = new List<Claim>
        {
            // JWT Standard Claims
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.FullName),
            new(JwtRegisteredClaimNames.Email, user.EmailAddress ?? string.Empty),

            // Issued At (Unix Timestamp)
            new(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),

            // Application Claims
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.MobilePhone, user.MobileNumber),
            new(ClaimTypes.Email, user.EmailAddress ?? string.Empty),

            // Used to invalidate existing JWTs
            new(TokenVersionClaim, user.TokenVersion.ToString())
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
    public string GenerateRefreshToken()    
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    
}