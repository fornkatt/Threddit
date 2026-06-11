using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Threddit.Contracts.Requests.Reports;
using Threddit.Contracts.Responses.Reports;
using Threddit.UI.ApiClient;

namespace Threddit.UI.Features.Shared;

public sealed partial class SubThreadReportsComponent : ComponentBase
{
    [Inject] private ThredditApiClient Client { get; set; } = null!;

    [Parameter, EditorRequired] public string SubThreadName { get; set; } = string.Empty;

    private IReadOnlyList<ReportApiDto> _reports = [];
    private bool _isLoading;
    private bool _isBusy;
    private string _statusFilter = "Pending";

    protected override async Task OnInitializedAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        _isLoading = true;
        StateHasChanged();

        var status = string.IsNullOrWhiteSpace(_statusFilter) ? null : _statusFilter;
        var result = await Client.GetSubThreadReportsAsync(SubThreadName, status);

        if (result.IsSuccess)
            _reports = result.Value!.Reports;

        _isLoading = false;
    }

    private async Task SetStatusAsync((Guid reportId, string status) args)
    {
        _isBusy = true;

        var result = await Client.SetReportStatusAsync(args.reportId, new SetReportStatusApiRequest(args.status));

        if (result.IsSuccess)
            await LoadAsync();

        _isBusy = false;
    }
}