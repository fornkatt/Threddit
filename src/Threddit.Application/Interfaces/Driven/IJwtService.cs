using Threddit.Domain.Entities;

namespace Threddit.Application.Interfaces.Driven;

public interface IJwtService
{
    /// <summary>Generates a JWT token for a user.</summary>
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}