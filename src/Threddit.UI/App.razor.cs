using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Threddit.UI.Interfaces;

namespace Threddit.UI;

public class AppBase : ComponentBase
{
    [Inject] public ICurrentUserStore CurrentUserStore { get; set; } = null!;
    [Inject] public AuthenticationStateProvider AuthState { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState.GetAuthenticationStateAsync();
        
        if (state.User.Identity?.IsAuthenticated == true)
            await CurrentUserStore.LoadAsync();
    }
}