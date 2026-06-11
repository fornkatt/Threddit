using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Responses.Reports;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Reports;

public sealed partial class SetReportStatusUseCase : ISetReportStatusUseCase
{
    private readonly IReportRepository _reportRepository;
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly ILogger<SetReportStatusUseCase> _logger;

    public SetReportStatusUseCase(
        IReportRepository reportRepository,
        ISubThreadRepository subThreadRepository,
        ILogger<SetReportStatusUseCase> logger
    )
    {
        _reportRepository = reportRepository;
        _subThreadRepository = subThreadRepository;
        _logger = logger;
    }

    public async Task<SetReportStatusResponse> ExecuteAsync(SetReportStatusRequest request)
    {
        var fetchResult = await _reportRepository.GetByIdAsync(request.ReportId);
        if (!fetchResult.IsSuccess)
        {
            LogFetchFailure(fetchResult.Exception, request.ReportId, fetchResult.ErrorMessage);
            var message = ResolveErrorMessage(fetchResult.ErrorType);
            return new SetReportStatusResponse(false, message, fetchResult.ErrorType);
        }

        var report = fetchResult.Value!;
        var isSitePrivileged = request.IsSiteAdmin || request.IsSiteOwner;
        var isSubThreadReport = report.Type is Report.ReportType.Post or Report.ReportType.Comment;

        if (isSubThreadReport)
        {
            var subThreadResult = await _subThreadRepository.GetByIdAsync(report.SubThreadId!.Value);
            if (!subThreadResult.IsSuccess)
            {
                LogSubThreadFetchFailure(subThreadResult.Exception, report.SubThreadId!.Value,
                    subThreadResult.ErrorMessage);
                var message = ResolveErrorMessage(subThreadResult.ErrorType);
                return new SetReportStatusResponse(false, message, subThreadResult.ErrorType);
            }

            var subThread = subThreadResult.Value!;
            var isModerator = request.ModeratedSubThreadIds.Contains(subThread.Id);
            var isSubThreadOwner = subThread.SubThreadOwner.UserId == request.RequestingUserId;

            if (!isModerator && !isSubThreadOwner && !isSitePrivileged)
            {
                LogUnauthorizedAttempt(request.RequestingUserId, report.Id);
                var message = ResolveErrorMessage(ErrorType.Forbidden);
                return new SetReportStatusResponse(false, message, ErrorType.Forbidden);
            }
        }
        else
        {
            if (!isSitePrivileged)
            {
                LogUnauthorizedAttempt(request.RequestingUserId, report.Id);
                var message = ResolveErrorMessage(ErrorType.Forbidden);
                return new SetReportStatusResponse(false, message, ErrorType.Forbidden);
            }
        }

        var setResult = report.SetStatus(request.NewStatus);
        if (!setResult.IsSuccess)
        {
            LogSameStatus(report.Id, request.NewStatus.ToString());
            var message = ResolveErrorMessage(setResult.ErrorType);
            return new SetReportStatusResponse(false, message, setResult.ErrorType);
        }

        var updateResult = await _reportRepository.UpdateAsync(report);
        if (!updateResult.IsSuccess)
        {
            LogUpdateFailure(updateResult.Exception, report.Id, updateResult.ErrorMessage);
            var message = ResolveErrorMessage(updateResult.ErrorType);
            return new SetReportStatusResponse(false, message, updateResult.ErrorType);
        }
        
        LogStatusUpdated(report.Id, request.NewStatus.ToString());
        return new SetReportStatusResponse(true, "Report status updated successfully.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.ReportNotFound => "Report not found.",
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.Forbidden => "You are not authorized to set status for this report.",
        ErrorType.SameStatus => "The report already has this status.",
        ErrorType.ConcurrencyFailure =>
            "Failed to update report due to internal server error. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}