using Microsoft.AspNetCore.Components;
using Threddit.Contracts.Requests.Auth;
using Threddit.UI.ApiClient;
using Threddit.UI.Auth;
using Threddit.UI.Features.Account.Models;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Features.Account;

public sealed partial class Login : ComponentBase
{
    [Inject] public ThredditApiClient Client { get; set; } = null!;
    [Inject] public ITokenStore TokenStore { get; set; } = null!;
    [Inject] public NavigationManager Nav { get; set; } = null!;
    [Inject] public JwtAuthenticationStateProvider AuthState { get; set; } = null!;
    [Inject] public ICurrentUserStore CurrentUserStore { get; set; } = null!;
    
    private readonly LoginForm _form = new();
    private string? _errorMessage;
    private bool _busy;

    private async Task HandleLoginAsync()
    {
        _busy = true;
        _errorMessage = null;

        var request = new LoginApiRequest(_form.UsernameOrEmail, _form.Password);
        var result = await Client.LoginAsync(request);

        if (result.IsSuccess)
        {
            await TokenStore.SetAsync(result.Value!);
            AuthState.NotifyUserAuthentication();
            await CurrentUserStore.LoadAsync();
            Nav.NavigateTo("/");
        }
        else
        {
            _errorMessage = result.ErrorMessage;
        }

        _busy = false;
    }
}