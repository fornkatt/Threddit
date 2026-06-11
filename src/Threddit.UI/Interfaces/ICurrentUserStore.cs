namespace Threddit.UI.Interfaces;

public interface ICurrentUserStore
{
    string? Username { get; }
    Guid? UserId { get; }

    bool IsSiteOwner { get; }
    bool IsSiteAdmin { get; }

    HashSet<Guid> SubscribedSubThreadIds { get; }
    List<string> SubscribedSubThreadNames { get; }

    HashSet<Guid> OwnedSubThreadIds { get; }
    HashSet<Guid> ModeratedSubThreadIds { get; }

    bool IsLoaded { get; }

    Task LoadAsync();
    void Clear();
    event Action? OnChanged;

    void AddSubscription(Guid subThreadId, string subThreadName);
    void RemoveSubscription(Guid subThreadId, string subThreadName);
}