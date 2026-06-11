using Threddit.Application.DTOs.Requests.Auth;
using Threddit.Application.DTOs.Responses.Auth;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IRegistrationUseCase
{
    /// <summary>Requests the registration of a new user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.EmailTaken"/></item>
    ///     <item><see cref="ErrorType.UsernameTaken"/></item>
    ///     <item><see cref="ErrorType.InvalidUsername"/></item>
    ///     <item><see cref="ErrorType.InvalidEmail"/></item>
    ///     <item><see cref="ErrorType.UsernameTooLong"/></item>
    ///     <item><see cref="ErrorType.EmailTooLong"/></item>
    ///     <item><see cref="ErrorType.RegistrationFailed"/></item>
    /// </list>
    /// </remarks>
    Task<RegistrationResponse> ExecuteAsync(RegistrationRequest request);
}