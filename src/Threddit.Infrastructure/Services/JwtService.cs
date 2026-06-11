using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Threddit.Application.Interfaces.Driven;
using Threddit.Domain.Entities;

namespace Threddit.Infrastructure.Services;

public sealed class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(
        IConfiguration configuration
    )
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(12);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            Subject = new ClaimsIdentity(BuildClaims(user)),
            Expires = expiresAt,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(descriptor);

        return (token, expiresAt);
    }

    private static List<Claim> BuildClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new("username", user.UserName!)
        };

        if (user.SiteOwner is not null)
            claims.Add(new Claim("role", "SiteOwner"));
        if (user.SiteAdmin is not null)
            claims.Add(new Claim("role", "SiteAdmin"));

        if (user.BannedSiteUser is not null && !user.BannedSiteUser.IsExpired)
            claims.Add(new Claim("banned", "true"));

        claims.AddRange(user.SubThreadModeratorRoles.Select(mod =>
            new Claim("moderator", mod.SubThreadId.ToString())));

        return claims;
    }
}