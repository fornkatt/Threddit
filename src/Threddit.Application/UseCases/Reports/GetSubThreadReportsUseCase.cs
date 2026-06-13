using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Responses.Reports;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Reports;

public sealed partial class GetSubThreadReportsUseCase : IGetSubThreadReportsUseCase
{
    private readonly IReportRepository _reportRepository;
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly ILogger<GetSubThreadReportsUseCase> _logger;

    public GetSubThreadReportsUseCase(
        IReportRepository reportRepository,
        ISubThreadRepository subThreadRepository,
        ILogger<GetSubThreadReportsUseCase> logger
    )
    {
        _reportRepository = reportRepository;
        _subThreadRepository = subThreadRepository;
        _logger = logger;
    }

    public async Task<GetReportsResponse> ExecuteAsync(GetSubThreadReportsRequest request)
    {
        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new GetReportsResponse(false, [], message, subThreadResult.ErrorType);
        }

        var subThread = subThreadResult.Value!;
        var isModerator = request.ModeratedSubThreadIds.Contains(subThread.Id);
        var isSubThreadOwner = request.OwnedSubThreadIds.Contains(subThread.Id);
        var isSitePrivileged = request.IsSiteAdmin || request.IsSiteOwner;

        if (!isModerator && !isSubThreadOwner && !isSitePrivileged)
        {
            LogUnauthorizedAccess(request.RequestingUserId, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new GetReportsResponse(false, [], message, ErrorType.Forbidden);
        }

        var reportsResult =
            await _reportRepository.GetBySubThreadAsync(subThread.Id, request.StatusFilter, request.Page,
                request.PageSize);
        if (!reportsResult.IsSuccess)
        {
            LogFetchFailure(reportsResult.Exception, subThread.Id, reportsResult.ErrorMessage);
            var message = ResolveErrorMessage(reportsResult.ErrorType);
            return new GetReportsResponse(false, [], message, reportsResult.ErrorType);
        }

        var reports = reportsResult.Value!;

        var dtos = reports.Select(MapToDto).ToImmutableList();
        return new GetReportsResponse(true, dtos, "Reports fetched successfully.");
    }

    private static ReportDto MapToDto(Report r) => new(
        r.Id, r.Type, r.Category, r.Status, r.Message,
        r.ReporterId, r.Reporter?.UserName,
        r.SubThreadId, r.SubThread?.Name,
        r.TargetPostId, r.TargetPost?.Title,
        r.TargetCommentId, r.TargetComment?.Content,
        null, null, null,
        null, null, null,
        r.ReportedAt
    );

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.Forbidden => "You are not authorized to view reports for this SubThread.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}