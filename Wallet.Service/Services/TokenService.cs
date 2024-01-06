using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Wallet.Domain.Enums;

namespace Wallet.Service.Services;

public class TokenService(string secretKey, string issuer, string audience)
{
    private readonly string _secretKey = secretKey;
    private readonly string _issuer = issuer;
    private readonly string _audience = audience;

    public string GenerateToken(string userId, string phone, int expirationMinutes = 60, TokenType type = TokenType.AccessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim("phone", phone),
        new Claim("type", type.ToString())
    }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _issuer,
            Audience = _audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            ClockSkew = TimeSpan.Zero // Use zero for clock skew for simplicity, adjust as needed
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch (Exception)
        {
            return null!; // Token validation failed
        }
    }

    public TokenType GetTokenType(string token)
    {
        var principal = this.ValidateToken(token);
        if (principal == null)
        {
            return TokenType.Error!;
        }
        var type = principal.FindFirst("type")?.Value;
        if (type == null)
        {
            return TokenType.Error!;
        }
        var returntype = Enum.Parse<TokenType>(type);
        return returntype;
    }

    public int GetUserId(string token)
    {
        var principal = this.ValidateToken(token);
        if (principal == null)
        {
            return -1;
        }
        var type = GetTokenType(token);
        if (type != TokenType.AccessToken) return -1!;
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int intUserId = int.Parse(userId);
        return intUserId;
    }

    public string GetPhone(string token)
    {
        var principal = this.ValidateToken(token);
        if (principal == null)
        {
            return null!;
        }
        var type = GetTokenType(token);
        if (type != TokenType.AccessToken) return null!;
        var phone = principal.FindFirst("phone")?.Value;
        return phone!;
    }
}
