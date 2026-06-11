using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.Users;

public sealed partial class GetCurrentUserSummaryUseCase : IGetCurrentUserSummaryUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetCurrentUserSummaryUseCase> _logger;

    public GetCurrentUserSummaryUseCase(
        IUserRepository userRepository,
        ILogger<GetCurrentUserSummaryUseCase> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetCurrentUserSummaryResponse> ExecuteAsync(GetCurrentUserSummaryRequest request)
    {
        var userResult = await _userRepository.GetCurrentUserSummaryAsync(request.UserId);
        if (!userResult.IsSuccess)
        {
            LogUserFetchFailure(userResult.Exception, request.UserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new GetCurrentUserSummaryResponse(false, message, userResult.ErrorType);
        }

        var user = userResult.Value!;

        var subscriptions = user.SubThreadSubscriptions
            .Select(s => new SubscribedSubThreadDto(s.SubThreadId, s.SubThread.Name))
            .ToImmutableList();

        var ownedIds = user.SubThreadOwnerRoles
            .Select(o => o.SubThreadId)
            .ToImmutableList();

        var moderatedIds = user.SubThreadModeratorRoles
            .Select(m => m.SubThreadId)
            .ToImmutableList();

        var summary = new CurrentUserSummaryDto(
            user.Id,
            user.UserName!,
            user.SiteOwner is not null,
            user.SiteAdmin is not null,
            subscriptions,
            ownedIds,
            moderatedIds
        );

        return new GetCurrentUserSummaryResponse(true, Summary: summary);
    }

    private static string ResolveErrorMessage(ErrorType? errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}