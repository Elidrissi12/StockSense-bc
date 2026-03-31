using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockManagement.Application.Interfaces;
using StockManagement.Domain.Enums;

namespace StockManagement.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _key;
    private readonly int _expiryMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        _issuer = configuration["Jwt:Issuer"] ?? "StockManagement";
        _audience = configuration["Jwt:Audience"] ?? "StockManagement";
        _key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key manquant dans la config.");

        if (!int.TryParse(configuration["Jwt:ExpiryMinutes"], out _expiryMinutes))
            _expiryMinutes = 60;
    }

    public Task<DateTime> GetTokenExpiryUtcAsync()
    {
        return Task.FromResult(DateTime.UtcNow.AddMinutes(_expiryMinutes));
    }

    public Task<string> GenerateTokenAsync(Guid userId, string email, UserRole role)
    {
        var expires = DateTime.UtcNow.AddMinutes(_expiryMinutes);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );

        var handler = new JwtSecurityTokenHandler();
        var tokenString = handler.WriteToken(token);
        return Task.FromResult(tokenString);
    }
}

