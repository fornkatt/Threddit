using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class GetSubThreadModeratorsUseCase : IGetSubThreadModeratorsUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly ILogger<GetSubThreadModeratorsUseCase> _logger;

    public GetSubThreadModeratorsUseCase(
        ISubThreadRepository subThreadRepository,
        ILogger<GetSubThreadModeratorsUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _logger = logger;
    }

    public async Task<GetSubThreadModeratorsResponse> ExecuteAsync(GetSubThreadModeratorsRequest request)
    {
        var result = await _subThreadRepository.GetModeratorsAsync(request.SubThreadName);
        if (!result.IsSuccess)
        {
            LogFetchFailure(result.Exception, request.SubThreadName, result.ErrorMessage);
            var message = ResolveErrorMessage(result.ErrorType);
            return new GetSubThreadModeratorsResponse(false, [], message, result.ErrorType);
        }

        var moderators = result.Value!
            .Select(m => new SubThreadModeratorDto(m.UserId, m.User.UserName!))
            .ToImmutableList();
        
        return new GetSubThreadModeratorsResponse(true, moderators);
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}