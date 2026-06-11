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

public sealed partial class GetSiteReportsUseCase : IGetSiteReportsUseCase
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<GetSiteReportsUseCase> _logger;

    public GetSiteReportsUseCase(
        IReportRepository reportRepository,
        ILogger<GetSiteReportsUseCase> logger
    )
    {
        _reportRepository = reportRepository;
        _logger = logger;
    }

    public async Task<GetReportsResponse> ExecuteAsync(GetSiteReportsRequest request)
    {
        var reportsResult =
            await _reportRepository.GetSiteWideAsync(request.StatusFilter, request.Page, request.PageSize);
        if (!reportsResult.IsSuccess)
        {
            LogFetchFailure(reportsResult.Exception, reportsResult.ErrorMessage);
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
        null, null,
        null, null,
        null, null,
        r.TargetUserId, r.TargetUser?.UserName,
        r.TargetSubThreadId, r.TargetSubThread?.Name,
        r.TargetDirectMessageId, r.TargetDirectMessage?.Content,
        r.ReportedAt
    );

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}