using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IDeleteSubThreadRuleUseCase
{
    /// <summary>Deletes an existing SubThread rule.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadRuleNotFound"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<DeleteSubThreadRuleResponse> ExecuteAsync(DeleteSubThreadRuleRequest request);
}