using Threddit.Application.DTOs.Requests.Auth;
using Threddit.Application.DTOs.Responses.Auth;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface ILoginUseCase
{
    /// <summary>Requests login for a user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.InvalidCredentials"/></item>
    ///     <item><see cref="ErrorType.Unknown"/></item>
    /// </list>
    /// </remarks>
    Task<LoginResponse> ExecuteAsync(LoginRequest request);
}