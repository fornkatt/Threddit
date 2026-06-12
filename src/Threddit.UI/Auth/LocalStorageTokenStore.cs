using Microsoft.JSInterop;
using Threddit.Contracts.Responses.Auth;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Auth;

public sealed class LocalStorageTokenStore : ITokenStore
{
    private const string TokenKey = "threddit.jwt";
    private const string ExpiryKey = "threddit.exp";
    private const string UserKey = "threddit.user";

    private readonly IJSRuntime _js;

    public LocalStorageTokenStore(
        IJSRuntime js
    )
    {
        _js = js;
    }

    public async ValueTask<string?> GetTokenAsync() =>
        await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

    public async ValueTask<string?> GetUsernameAsync() =>
        await _js.InvokeAsync<string?>("localStorage.getItem", UserKey);


    public async ValueTask SetAsync(LoginApiResponse login)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, login.JwtToken);
        await _js.InvokeVoidAsync("localStorage.setItem", ExpiryKey, login.ExpiresAt.ToString("o"));
        await _js.InvokeVoidAsync("localStorage.setItem", UserKey, login.Username);
    }

    public async ValueTask ClearAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await _js.InvokeVoidAsync("localStorage.removeItem", ExpiryKey);
        await _js.InvokeVoidAsync("localStorage.removeItem", UserKey);
    }
}