using Microsoft.AspNetCore.Components;
using Threddit.Contracts.Requests.SubThreads;
using Threddit.Contracts.Responses.SubThreads;
using Threddit.UI.ApiClient;
using Threddit.UI.Auth;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Shared;

public sealed partial class TopNavBar : ComponentBase, IDisposable
{
    [Inject] public ThredditApiClient Client { get; set; } = null!;
    [Inject] public ITokenStore TokenStore { get; set; } = null!;
    [Inject] public NavigationManager Nav { get; set; } = null!;
    [Inject] public JwtAuthenticationStateProvider AuthState { get; set; } = null!;
    [Inject] public ICurrentUserStore CurrentUserStore { get; set; } = null!;


    private string _searchInput = string.Empty;
    private List<GetSubThreadSearchApiResponse> _searchResults = [];

    private bool _isSearchOpen;
    private bool _isSearchBusy;

    private CancellationTokenSource? _searchDebounce;

    protected override void OnInitialized()
    {
        CurrentUserStore.OnChanged += StateHasChanged;
    }

    public void Dispose()
    {
        CurrentUserStore.OnChanged -= StateHasChanged;
    }

    private async Task LogoutAsync()
    {
        await Client.LogoutAsync();
        await TokenStore.ClearAsync();
        AuthState.NotifyUserLogout();
        CurrentUserStore.Clear();

        Nav.NavigateTo("/");
    }

    private async Task OnSearchFocusAsync()
    {
        _isSearchOpen = true;

        await FetchResultsAsync(_searchInput);
    }

    private async Task OnSearchInputAsync(ChangeEventArgs e)
    {
        _searchInput = e.Value?.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(_searchInput))
            _searchResults.Clear();

        _searchDebounce?.Cancel();
        _searchDebounce = new CancellationTokenSource();
        var token = _searchDebounce.Token;

        try
        {
            await Task.Delay(300, token);
            await FetchResultsAsync(_searchInput);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task FetchResultsAsync(string query)
    {
        _isSearchBusy = true;
        StateHasChanged();

        var result = await Client.SearchAsync(new GetSubThreadsBySearchApiRequest(query, PageSize: 5));

        if (result.IsSuccess)
            _searchResults = [..result.Value!.Items];

        _isSearchBusy = false;
        StateHasChanged();
    }

    private void NavigateToSubThread(string name)
    {
        _isSearchOpen = false;
        _searchInput = string.Empty;
        _searchResults.Clear();
        Nav.NavigateTo($"/t/{name}");
    }

    private void OnSearchBlur()
    {
        Task.Delay(150).ContinueWith(_ =>
        {
            _isSearchOpen = false;
            InvokeAsync(StateHasChanged);
        });
    }
}