using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Threddit.Contracts.Requests.Conversations;
using Threddit.Contracts.Responses.Conversations;
using Threddit.UI.ApiClient;

namespace Threddit.UI.Features.Conversations;

public sealed partial class ConversationView : ComponentBase, IDisposable
{
    [Inject] private ThredditApiClient Client { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    [SupplyParameterFromQuery(Name = "with")]
    public string? PendingUsername { get; set; }

    [Parameter] public Guid ConversationId { get; set; } = Guid.Empty;

    private List<DirectMessageApiDto> _messages = [];
    private bool _isLoading;
    private bool _isBusy;
    private string _messageContent = string.Empty;
    private string? _sendErrorMessage;
    private DirectMessageApiDto? _replyTo;

    private bool _isGroup;
    private string? _groupName;
    private bool _showAddMember;
    private string _addMemberUsername = string.Empty;
    private string? _addMemberErrorMessage;

    private IReadOnlyList<GroupMemberApiDto> _groupMembers = [];
    private bool _isCreator;
    private Guid _creatorId;
    private bool _showMembers;
    private string? _memberActionErrorMessage;

    private bool _isPendingConversation => !string.IsNullOrWhiteSpace(PendingUsername) && ConversationId == Guid.Empty;

    protected override async Task OnParametersSetAsync()
    {
        if (_isPendingConversation)
        {
            _isGroup = false;
            _isLoading = false;
            return;
        }

        _isGroup = Nav.Uri.Contains("/conversations/group");
        _isLoading = true;

        var result = _isGroup
            ? await Client.GetGroupMessagesAsync(ConversationId)
            : await Client.GetConversationMessagesAsync(ConversationId);

        if (result.IsSuccess)
            _messages = result.Value!.Messages;

        if (_isGroup)
        {
            var allResult = await Client.GetConversationsAsync();
            var group = allResult.Value?.GroupConversations
                .FirstOrDefault(g => g.Id == ConversationId);

            _groupName = group?.Name;
            _groupMembers = group?.Members.Where(m => !m.HasLeft).ToList() ?? [];

            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var currentUserIdString = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(currentUserIdString, out var currentUserId))
            {
                _creatorId = group?.CreatedById ?? Guid.Empty;
                _isCreator = group?.CreatedById == currentUserId;
            }
        }

        _isLoading = false;
    }

    public void Dispose() => _messages = [];

    private void NavigateBack() => Nav.NavigateTo("/conversations");

    private void SetReply(DirectMessageApiDto message) => _replyTo = message;

    private async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(_messageContent) || _isBusy)
            return;

        _isBusy = true;
        _sendErrorMessage = null;
        StateHasChanged();

        if (_isPendingConversation)
        {
            var createResult = await Client.StartConversationAsync(PendingUsername!);
            if (!createResult.IsSuccess)
            {
                _sendErrorMessage = createResult.ErrorMessage ?? "Failed to start conversation.";
                _isBusy = false;
                return;
            }

            Nav.NavigateTo($"/conversations/{createResult.Value}", replace: true);
            ConversationId = createResult.Value;
            PendingUsername = null;
        }

        var request = new SendMessageApiRequest(_messageContent, _replyTo?.Id);
        var result = _isGroup
            ? await Client.SendGroupMessageAsync(ConversationId, request)
            : await Client.SendMessageAsync(ConversationId, request);

        if (result.IsSuccess)
        {
            var sent = result.Value!;

            if (_replyTo is not null && sent.ParentMessageSenderUsername is null)
                sent = sent with { ParentMessageSenderUsername = _replyTo.SenderUsername };

            _messages.Add(sent);
            _messageContent = string.Empty;
            _replyTo = null;
        }
        else
        {
            _sendErrorMessage = result.ErrorMessage ?? "Failed to send message.";
        }

        _isBusy = false;
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e is { Key: "Enter", ShiftKey: false })
            await SendAsync();
    }

    private async Task AddMemberAsync()
    {
        _addMemberErrorMessage = null;

        if (string.IsNullOrWhiteSpace(_addMemberUsername))
            return;

        _isBusy = true;
        StateHasChanged();

        var profileResult = await Client.GetUserProfileAsync(_addMemberUsername.Trim());
        if (!profileResult.IsSuccess)
        {
            _addMemberErrorMessage = $"User '{_addMemberUsername}' not found.'";
            _isBusy = false;
            return;
        }

        var request = new AddGroupMemberApiRequest(profileResult.Value!.Id);
        var result = await Client.AddGroupMemberAsync(ConversationId, request);
        if (result.IsSuccess)
        {
            _showAddMember = false;
            _addMemberUsername = string.Empty;
            await OnParametersSetAsync();
        }
        else
        {
            _addMemberErrorMessage = result.ErrorMessage ?? "Failed to add member.";
        }

        _isBusy = false;
    }

    private async Task LeaveGroupAsync()
    {
        _memberActionErrorMessage = null;
        _isBusy = true;
        StateHasChanged();

        var result = await Client.LeaveGroupAsync(ConversationId);
        if (result.IsSuccess)
            Nav.NavigateTo("/conversations");
        else
            _memberActionErrorMessage = result.ErrorMessage ?? "Failed to leave group.";

        _isBusy = false;
    }

    private async Task RemoveMemberAsync(Guid targetMemberId)
    {
        _memberActionErrorMessage = null;
        _isBusy = true;
        StateHasChanged();

        var result = await Client.RemoveGroupMemberAsync(ConversationId, targetMemberId);
        if (result.IsSuccess)
            await OnParametersSetAsync();
        else
            _memberActionErrorMessage = result.ErrorMessage ?? "Failed to remove member.";

        _isBusy = false;
    }
    
    private void NavigateToUserProfile(string? username)
    {
        if (username is not null)
            Nav.NavigateTo($"/u/{username}");
    }
}