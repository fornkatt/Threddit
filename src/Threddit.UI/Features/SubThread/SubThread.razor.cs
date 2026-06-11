using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Threddit.Contracts.Responses.SubThreads;
using Threddit.UI.ApiClient;
using Threddit.UI.Features.SubThread.Models;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Features.SubThread;

public sealed partial class SubThread : ComponentBase, IAsyncDisposable
{
    [Parameter] public string SubThreadName { get; set; } = string.Empty;

    [Inject] private ThredditApiClient Client { get; set; } = null!;
    [Inject] private ICurrentUserStore CurrentUserStore { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private SubThreadViewModel? _subThread;
    private readonly List<GetSubThreadPostApiResponse> _posts = [];

    private string? _previousSubThreadName;

    private bool _isLoadingSubThread;
    private bool _isLoadingMorePosts;
    private bool _isSubscribeBusy;
    private bool _observerAttached;

    private string? _errorMessage;

    private int _currentPage = 1;
    private const int PageSize = 10;
    private bool _hasNextPage;

    private bool IsSubscribed => CurrentUserStore.SubscribedSubThreadIds.Contains(_subThread!.Id);

    private ElementReference _scrollSentinel;
    private IJSObjectReference? _observer;
    private DotNetObjectReference<SubThread>? _dotNetRef;

    public async ValueTask DisposeAsync()
    {
        if (_observer is not null)
            await _observer.DisposeAsync();

        _dotNetRef?.Dispose();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        if (!_observerAttached && !_isLoadingSubThread && _subThread is not null)
        {
            _observerAttached = true;
            _observer = await JS.InvokeAsync<IJSObjectReference>(
                "infiniteScroll.observe", _scrollSentinel, _dotNetRef);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_previousSubThreadName == SubThreadName)
            return;

        _previousSubThreadName = SubThreadName;

        if (_observer is not null)
        {
            await _observer.DisposeAsync();
            _observer = null;
        }

        _observerAttached = false;
        _currentPage = 1;

        await LoadAsync();
    }

    [JSInvokable]
    public async Task OnSentinelVisible()
    {
        if (_isLoadingMorePosts || !_hasNextPage) return;

        _isLoadingMorePosts = true;
        StateHasChanged();
        await LoadMoreAsync();
    }

    private async Task LoadAsync()
    {
        _isLoadingSubThread = true;
        _errorMessage = null;

        var subThreadResult = await Client.GetSubThreadByName(SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            _errorMessage = subThreadResult.ErrorMessage;
            _isLoadingSubThread = false;
            return;
        }

        _subThread = SubThreadViewModel.FromResponse(subThreadResult.Value!);

        var postsResult = await Client.GetSubThreadPostsAsync(SubThreadName, _currentPage, PageSize);
        if (!postsResult.IsSuccess)
        {
            _errorMessage = postsResult.ErrorMessage ?? "Failed to load posts.";
            _isLoadingSubThread = false;
            return;
        }

        _posts.Clear();
        _posts.AddRange(postsResult.Value!.Items);
        _hasNextPage = postsResult.Value.HasNextPage;
        _currentPage = 1;
        _isLoadingSubThread = false;
    }

    private async Task HandleSubscribeAsync()
    {
        if (_isSubscribeBusy || _subThread is null)
            return;

        _isSubscribeBusy = true;
        _errorMessage = null;

        var currentlySubscribed = IsSubscribed;
        var result = await Client.SubscribeToSubThreadAsync(_subThread.Name, currentlySubscribed);

        if (result.IsSuccess)
        {
            if (currentlySubscribed)
                CurrentUserStore.RemoveSubscription(_subThread.Id, _subThread.Name);
            else
                CurrentUserStore.AddSubscription(_subThread.Id, _subThread.Name);
        }
        else
        {
            _errorMessage = result.ErrorMessage;
        }

        _isSubscribeBusy = false;
    }

    private async Task LoadMoreAsync()
    {
        _currentPage++;

        var result = await Client.GetSubThreadPostsAsync(SubThreadName, _currentPage, PageSize);
        if (result.IsSuccess)
        {
            _posts.AddRange(result.Value!.Items);
            _hasNextPage = result.Value.HasNextPage;
        }

        _isLoadingMorePosts = false;
        StateHasChanged();
    }
    
    private void NavigateToUserProfile(string? username)
    {
        if (username is not null)
            Nav.NavigateTo($"/u/{username}");
    }

    private bool CanDeletePost(GetSubThreadPostApiResponse post)
    {
        if (!CurrentUserStore.IsLoaded || _subThread is null)
            return false;
        if (CurrentUserStore.IsSiteOwner || CurrentUserStore.IsSiteAdmin)
            return true;
        if (CurrentUserStore.ModeratedSubThreadIds.Contains(_subThread.Id))
            return true;
        if (CurrentUserStore.OwnedSubThreadIds.Contains(_subThread.Id))
            return true;

        return post.PostedById == CurrentUserStore.UserId;
    }

    private bool CanEditPost(GetSubThreadPostApiResponse post)
    {
        if (!CurrentUserStore.IsLoaded)
            return false;

        return post.PostedById == CurrentUserStore.UserId;
    }
}