using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class DeleteSubThreadUseCase : IDeleteSubThreadUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly ILogger<DeleteSubThreadUseCase> _logger;

    public DeleteSubThreadUseCase(
        ISubThreadRepository subThreadRepository,
        ILogger<DeleteSubThreadUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _logger = logger;
    }

    public async Task<DeleteSubThreadResponse> ExecuteAsync(DeleteSubThreadRequest request)
    {
        var fetchResult = await _subThreadRepository.GetByIdAsync(request.SubThreadId);
        if (!fetchResult.IsSuccess)
        {
            LogFetchFailure(fetchResult.Exception, request.SubThreadId, fetchResult.ErrorMessage);
            var message = ResolveErrorMessage(fetchResult.ErrorType);
            return new DeleteSubThreadResponse(false, message, fetchResult.ErrorType);
        }

        var subThread = fetchResult.Value!;

        var isSitePrivileged = request.IsSiteAdmin || request.IsSiteOwner;
        var isSubThreadOwner = subThread.SubThreadOwner.UserId == request.RequestingUserId;

        if (!isSitePrivileged && !isSubThreadOwner)
        {
            LogUnauthorizedDeletionAttempt(request.RequestingUserId, request.SubThreadId);
            return new DeleteSubThreadResponse(false,
                "You are not authorized to delete this SubThread.", ErrorType.Forbidden);
        }
        
        var deleteResult = await _subThreadRepository.DeleteAsync(subThread);
        if (!deleteResult.IsSuccess)
        {
            LogDeletionFailure(deleteResult.Exception, request.SubThreadId, deleteResult.ErrorMessage);
            var message = ResolveErrorMessage(deleteResult.ErrorType);
            return new DeleteSubThreadResponse(false, message, deleteResult.ErrorType);
        }
        
        LogDeletionSuccess(subThread.Name, subThread.Id);
        return new DeleteSubThreadResponse(true, "SubThread deletion successful.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        ErrorType.ConcurrencyFailure => "The SubThread may have been modified by another user. Please refresh and try again.",
        _ => "An unexpected error occurred trying to delete SubThread. Please try again later."
    };
}