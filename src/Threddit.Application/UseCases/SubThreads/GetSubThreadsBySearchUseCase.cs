using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class GetSubThreadsBySearchUseCase : IGetSubThreadsBySearchUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly ILogger<GetSubThreadsBySearchUseCase> _logger;

    public GetSubThreadsBySearchUseCase(
        ISubThreadRepository subThreadRepository,
        ILogger<GetSubThreadsBySearchUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _logger = logger;
    }

    public async Task<GetSubThreadsBySearchResponse> ExecuteAsync(GetSubThreadsBySearchRequest request)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var result = string.IsNullOrWhiteSpace(request.Query)
            ? await _subThreadRepository.GetTopAsync(page, pageSize)
            : await _subThreadRepository.SearchAsync(request.Query.Trim(), page, pageSize);

        if (!result.IsSuccess)
        {
            LogSearchFailure(result.Exception, request.Query, result.ErrorMessage);
            return new GetSubThreadsBySearchResponse(false, Message: "Search failed. Please try again.",
                ErrorType: result.ErrorType);
        }

        var paged = result.Value!;

        var dto = new PagedResult<SubThreadSummaryDto>(
            paged.Items.Select(s => new SubThreadSummaryDto(
                s.Id,
                s.Name,
                s.SubscriberCount
            )).ToList().AsReadOnly(),
            paged.Page,
            paged.PageSize,
            paged.TotalCount
        );

        return new GetSubThreadsBySearchResponse(true, dto);
    }
}