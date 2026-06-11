using Threddit.UI.ApiClient;
using Threddit.UI.Auth;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Stores;

public sealed class CurrentUserStore : ICurrentUserStore
{
    private readonly ThredditApiClient _client;
    private readonly ITokenStore _tokenStore;

    public string? Username { get; private set; }
    public Guid? UserId { get; private set; }

    public bool IsSiteOwner { get; private set; }
    public bool IsSiteAdmin { get; private set; }

    public HashSet<Guid> SubscribedSubThreadIds { get; } = [];
    public List<string> SubscribedSubThreadNames { get; } = [];

    public HashSet<Guid> OwnedSubThreadIds { get; } = [];
    public HashSet<Guid> ModeratedSubThreadIds { get; } = [];

    public bool IsLoaded { get; private set; }

    public event Action? OnChanged;

    public CurrentUserStore(
        ThredditApiClient client,
        ITokenStore tokenStore
    )
    {
        _client = client;
        _tokenStore = tokenStore;
    }

    public async Task LoadAsync()
    {
        var token = await _tokenStore.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return;

        var result = await _client.GetCurrentUserSummaryAsync();
        if (!result.IsSuccess)
            return;

        var data = result.Value!;

        Username = data.Username;
        UserId = data.UserId;
        IsSiteOwner = data.IsSiteOwner;
        IsSiteAdmin = data.IsSiteAdmin;

        SubscribedSubThreadIds.Clear();
        SubscribedSubThreadNames.Clear();
        OwnedSubThreadIds.Clear();
        ModeratedSubThreadIds.Clear();

        foreach (var sub in data.Subscriptions)
        {
            SubscribedSubThreadIds.Add(sub.Id);
            SubscribedSubThreadNames.Add(sub.Name);
        }

        SubscribedSubThreadNames.Sort(StringComparer.OrdinalIgnoreCase);

        foreach (var id in data.OwnedSubThreadIds)
            OwnedSubThreadIds.Add(id);

        foreach (var id in data.ModeratedSubThreadIds)
            ModeratedSubThreadIds.Add(id);

        IsLoaded = true;
        OnChanged?.Invoke();
    }

    public void Clear()
    {
        Username = null;
        UserId = null;
        IsSiteOwner = false;
        IsSiteAdmin = false;
        SubscribedSubThreadIds.Clear();
        SubscribedSubThreadNames.Clear();
        OwnedSubThreadIds.Clear();
        ModeratedSubThreadIds.Clear();
        IsLoaded = false;
        OnChanged?.Invoke();
    }

    public void AddSubscription(Guid subThreadId, string subThreadName)
    {
        if (!SubscribedSubThreadIds.Add(subThreadId))
            return;

        SubscribedSubThreadNames.Add(subThreadName);
        SubscribedSubThreadNames.Sort(StringComparer.OrdinalIgnoreCase);
        OnChanged?.Invoke();
    }

    public void RemoveSubscription(Guid subThreadId, string subThreadName)
    {
        if (!SubscribedSubThreadIds.Remove(subThreadId))
            return;

        SubscribedSubThreadNames.Remove(subThreadName);
        OnChanged?.Invoke();
    }
}