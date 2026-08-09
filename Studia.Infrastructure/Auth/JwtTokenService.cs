using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Studia.Application.Auth;
using Studia.Domain.Users;

namespace Studia.Infrastructure.Auth;

public class JwtTokenService : IJwtTokenService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);

    private readonly SymmetricSecurityKey _signingKey;
    private readonly string _issuer;

    public JwtTokenService(string signingSecret, string issuer = "Studia.LMS")
    {
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingSecret));
        _issuer = issuer;
    }

    public GeneratedToken Generate(Guid userId, string email, Role role)
    {
        var jti = Guid.NewGuid().ToString();
        var expiresAtUtc = DateTime.UtcNow.Add(TokenLifetime);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new GeneratedToken(tokenString, jti, expiresAtUtc);
    }

    public DecodedToken? Decode(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            var userId = Guid.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
            var email = principal.FindFirst(JwtRegisteredClaimNames.Email)!.Value;
            var role = Enum.Parse<Role>(principal.FindFirst(ClaimTypes.Role)!.Value);
            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)!.Value;

            return new DecodedToken(userId, email, role, jti, validatedToken.ValidTo);
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException or NullReferenceException)
        {
            return null;
        }
    }
}
