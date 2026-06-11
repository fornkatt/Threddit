using Microsoft.AspNetCore.Components;
using Threddit.Contracts.Responses.Reports;

namespace Threddit.UI.Features.Shared;

public sealed partial class ReportItemComponent : ComponentBase
{
    [Parameter, EditorRequired] public ReportApiDto Report { get; set; } = null!;
    [Parameter] public bool IsBusy { get; set; }
    [Parameter] public EventCallback<(Guid Id, string Status)> OnSetStatus { get; set; }
    
    private Task MarkReceivedAsync()  => OnSetStatus.InvokeAsync((Report.Id, "Received"));
    private Task MarkDismissedAsync() => OnSetStatus.InvokeAsync((Report.Id, "Dismissed"));
    private Task MarkResolvedAsync()  => OnSetStatus.InvokeAsync((Report.Id, "Resolved"));
}