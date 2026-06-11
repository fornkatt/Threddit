using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Shared;

public sealed partial class LeftSidebar : ComponentBase
{
    [Inject] private ICurrentUserStore CurrentUserStore { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private bool _isAdminSectionOpen = true;

    private void ToggleAdminSection() => _isAdminSectionOpen = !_isAdminSectionOpen;

    private string? CurrentSubThreadName
    {
        get
        {
            var uri = new Uri(Nav.Uri);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length >= 2 && segments[0] == "t")
                return segments[1];

            return null;
        }
    }

    private string AdminReportsHref =>
        CurrentSubThreadName is not null
            ? $"/admin/reports?subthread={Uri.EscapeDataString(CurrentSubThreadName)}"
            : "/admin/reports";

    protected override void OnInitialized()
    {
        CurrentUserStore.OnChanged += StateHasChanged;
        Nav.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => StateHasChanged();

    public void Dispose()
    {
        CurrentUserStore.OnChanged -= StateHasChanged;
        Nav.LocationChanged -= OnLocationChanged;
    }
}