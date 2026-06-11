using Microsoft.AspNetCore.Components;
using Threddit.Contracts.Requests.Conversations;
using Threddit.Contracts.Responses.Conversations;
using Threddit.UI.ApiClient;

namespace Threddit.UI.Features.Conversations;

public sealed partial class ConversationsPage : ComponentBase, IDisposable
{
    [Inject] private ThredditApiClient Client { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private IReadOnlyList<ConversationApiDto> _conversations = [];
    private IReadOnlyList<GroupConversationApiDto> _groups = [];
    private bool _isLoading;
    private bool _isBusy;
    private bool _showNewGroup;
    private string _newGroupName = string.Empty;
    private string _newGroupUsernames = string.Empty;
    private string? _errorMessage;
    private string? _groupErrorMessage;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;

        var result = await Client.GetConversationsAsync();

        if (result.IsSuccess)
        {
            _conversations = result.Value!.Conversations;
            _groups = result.Value!.GroupConversations;
        }
        else
        {
            _errorMessage = result.ErrorMessage ?? "Failed to load conversations.";
        }

        _isLoading = false;
    }

    public void Dispose()
    {
        _conversations = [];
        _groups = [];
    }

    private void OpenConversation(Guid id, bool isGroup)
    {
        Nav.NavigateTo(isGroup ? $"/conversations/group/{id}" : $"/conversations/{id}");
    }

    private async Task CreateGroupAsync()
    {
        _groupErrorMessage = null;

        if (string.IsNullOrWhiteSpace(_newGroupName))
        {
            _groupErrorMessage = "Group name is required.";
            return;
        }

        _isBusy = true;
        StateHasChanged();

        var usernames = _newGroupUsernames
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var memberIds = new List<Guid>();

        foreach (var username in usernames)
        {
            var profileResult = await Client.GetUserProfileAsync(username);
            if (!profileResult.IsSuccess)
            {
                _groupErrorMessage = $"User '{username}' not found.";
                _isBusy = false;
                return;
            }

            memberIds.Add(profileResult.Value!.Id);
        }

        var result = await Client
            .CreateGroupConversationAsync(new CreateGroupConversationApiRequest(_newGroupName, memberIds));

        if (result.IsSuccess)
        {
            _showNewGroup = false;
            Nav.NavigateTo($"/conversations/group/{result.Value}");
        }
        else
        {
            _groupErrorMessage = result.ErrorMessage ?? "Failed to create group.";
        }

        _isBusy = false;
    }
}