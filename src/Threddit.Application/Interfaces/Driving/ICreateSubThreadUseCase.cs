using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface ICreateSubThreadUseCase
{
    /// <summary>Requests the creation of a new SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.NameEmpty"/></item>
    ///     <item><see cref="ErrorType.NameTooLong"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.ImageUrlTooLong"/></item>
    ///     <item><see cref="ErrorType.AlreadyAssigned"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<CreateSubThreadResponse> ExecuteAsync(CreateSubThreadRequest request);
}