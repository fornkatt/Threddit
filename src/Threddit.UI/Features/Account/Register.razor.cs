using Microsoft.AspNetCore.Components;
using Threddit.Contracts.Requests.Auth;
using Threddit.UI.ApiClient;
using Threddit.UI.Features.Account.Models;

namespace Threddit.UI.Features.Account;

public sealed partial class Register : ComponentBase
{
    [Inject] public ThredditApiClient Client { get; set; } = null!;
    [Inject] public NavigationManager Nav { get; set; } = null!;
    
    private readonly RegisterForm _form = new();
    private string? _errorMessage;
    private bool _busy;

    private async Task HandleRegistrationAsync()
    {
        _busy = true;
        _errorMessage = null;

        var request = new RegistrationApiRequest(_form.Username, _form.Email, _form.Password);
        var result = await Client.RegisterAsync(request);

        if (result.IsSuccess)
        {
            Nav.NavigateTo("/login");
        }
        else
        {
            _errorMessage = result.ErrorMessage;
        }
        
        _busy = false;
    }
}