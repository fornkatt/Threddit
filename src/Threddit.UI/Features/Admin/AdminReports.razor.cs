using Microsoft.AspNetCore.Components;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Features.Admin;

public sealed partial class AdminReports : ComponentBase
{
    [Inject] private ICurrentUserStore CurrentUserStore { get; set; } = null!;
    
    [SupplyParameterFromQuery(Name = "subthread")]
    public string? SubThreadName { get; set; }
}