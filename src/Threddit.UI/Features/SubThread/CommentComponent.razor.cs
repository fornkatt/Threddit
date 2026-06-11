using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Threddit.Contracts.Requests.Comments;
using Threddit.Contracts.Requests.Reports;
using Threddit.Contracts.Responses.Comments;
using Threddit.UI.ApiClient;
using Threddit.UI.Features.SubThread.Models;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Features.SubThread;

public sealed partial class CommentComponent : ComponentBase
{
    [Parameter, EditorRequired] public GetCommentApiResponse Comment { get; set; } = null!;
    [Parameter, EditorRequired] public string SubThreadName { get; set; } = string.Empty;
    [Parameter, EditorRequired] public Guid SubThreadId { get; set; }
    [Parameter, EditorRequired] public Guid PostId { get; set; }

    [Inject] private ThredditApiClient Client { get; set; } = null!;
    [Inject] private ICurrentUserStore CurrentUserStore { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private List<GetCommentApiResponse> _replies = [];

    private bool _isRepliesLoaded;
    private bool _isLoadingReplies;

    private bool _isVoteBusy;

    private bool _isShowingReplyBox;
    private readonly CreateCommentForm _replyForm = new();
    private bool _isReplyBusy;
    private string? _replyErrorMessage;

    private bool _isDeletePending;
    private string _deleteReason = string.Empty;
    private bool _isDeleteBusy;
    private string? _deleteErrorMessage;

    private bool _isEditPending;
    private string _editContent = string.Empty;
    private bool _isEditBusy;
    private string? _editErrorMessage;

    private bool _isReportPending;
    private string _reportCategory = "Spam";
    private string _reportMessage = string.Empty;
    private bool _isReportBusy;
    private string? _reportErrorMessage;
    private bool _isReportSuccess;

    private bool _isCommentImageUploadBusy;
    private string? _commentImageUploadErrorMessage;

    private IBrowserFile? _pendingImageFile;
    private string? _imagePreviewUrl;

    private void ToggleReplyBox() => _isShowingReplyBox = !_isShowingReplyBox;

    private async Task OnCommentImageSelectedAsync(InputFileChangeEventArgs e)
    {
        _pendingImageFile = e.File;
        _commentImageUploadErrorMessage = null;

        var buffer = new byte[_pendingImageFile.Size];
        await _pendingImageFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).ReadExactlyAsync(buffer);
        _imagePreviewUrl = $"data:{_pendingImageFile.ContentType};base64,{Convert.ToBase64String(buffer)}";
    }

    protected override void OnInitialized()
    {
        if (Comment.Replies.Count > 0)
        {
            _replies = [..Comment.Replies];
            _isRepliesLoaded = true;
        }
    }

    private async Task LoadRepliesAsync()
    {
        _isLoadingReplies = true;

        var result = await Client.GetCommentRepliesAsync(SubThreadName, PostId, Comment.Id);

        if (result.IsSuccess)
        {
            _replies = [..result.Value!];
            _isRepliesLoaded = true;
        }

        _isLoadingReplies = false;
    }

    private async Task VoteCommentAsync(bool isUpvote)
    {
        if (_isVoteBusy)
            return;

        _isVoteBusy = true;

        var result = await Client.VoteCommentAsync(SubThreadName, PostId, Comment.Id, isUpvote);

        if (result.IsSuccess)
            Comment = Comment with { Score = result.Value };

        _isVoteBusy = false;
    }

    private async Task HandleReplyAsync()
    {
        _isReplyBusy = true;
        _replyErrorMessage = null;

        if (_pendingImageFile is not null)
        {
            _isCommentImageUploadBusy = true;
            StateHasChanged();
            await using var stream = _pendingImageFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            var uploadResult = await Client
                .UploadImageAsync(stream, _pendingImageFile.Name, _pendingImageFile.ContentType, "comment");
            _isCommentImageUploadBusy = false;

            if (uploadResult.IsSuccess)
                _replyForm.ImageUrl = uploadResult.Value!;
            else
            {
                _commentImageUploadErrorMessage = uploadResult.ErrorMessage ?? "Upload failed.";
                _isReplyBusy = false;
                return;
            }
        }

        var request = new CreateCommentApiRequest(_replyForm.Content, Comment.Id, _replyForm.ImageUrl);
        var result = await Client.CreateCommentAsync(SubThreadName, PostId, request);

        if (result.IsSuccess)
        {
            var r = result.Value!;
            _replies.Add(new GetCommentApiResponse(
                r.Id,
                r.CommentedById,
                r.CommentedByUsername,
                r.CommentedByProfilePicture,
                r.Content,
                r.ImageUrl,
                r.Score,
                r.CommentedAt,
                null,
                r.IsDeleted,
                [],
                r.HasReplies
            ));
            _isRepliesLoaded = true;
            _replyForm.Content = string.Empty;
            _replyForm.ImageUrl = string.Empty;
            _pendingImageFile = null;
            _imagePreviewUrl = null;
            _isShowingReplyBox = false;
        }
        else
        {
            _replyErrorMessage = result.ErrorMessage;
        }

        _isReplyBusy = false;
    }

    private void RequestDelete()
    {
        _isDeletePending = true;
        _deleteReason = string.Empty;
        _deleteErrorMessage = null;
    }

    private void CancelDelete()
    {
        _isDeletePending = false;
        _deleteReason = string.Empty;
    }

    private async Task ConfirmDeleteAsync()
    {
        _isDeleteBusy = true;
        _deleteErrorMessage = null;

        var reason = (IsModeratorDelete && !IsAuthorDelete) ? _deleteReason : null;
        var result = await Client.DeleteCommentAsync(SubThreadName, PostId, Comment.Id, reason);

        if (result.IsSuccess)
        {
            Comment = Comment with { IsDeleted = true, Content = null };
            _isDeletePending = false;
        }
        else
        {
            _deleteErrorMessage = result.ErrorMessage;
        }

        _isDeleteBusy = false;
    }

    private void RequestEdit()
    {
        _isEditPending = true;
        _editContent = Comment.Content ?? string.Empty;
        _editErrorMessage = null;
    }

    private void CancelEdit()
    {
        _isEditPending = false;
        _editContent = string.Empty;
    }

    private async Task ConfirmEditAsync()
    {
        _isEditBusy = true;
        _editErrorMessage = null;

        var result = await Client.EditCommentAsync(SubThreadName, PostId, Comment.Id,
            new EditCommentApiRequest(_editContent));

        if (result.IsSuccess)
        {
            Comment = Comment with { Content = _editContent };
            _isEditPending = false;
        }
        else
        {
            _editErrorMessage = result.ErrorMessage;
        }

        _isEditBusy = false;
    }

    private void RequestReport()
    {
        _isReportPending = true;
        _reportCategory = "Spam";
        _reportMessage = string.Empty;
        _reportErrorMessage = null;
        _isReportSuccess = false;
    }

    private void CancelReportComment() => _isReportPending = false;

    private async Task ConfirmReportCommentAsync()
    {
        _isReportBusy = true;
        _reportErrorMessage = null;

        var request = new CreateSubThreadReportApiRequest(
            "Comment",
            Comment.Id,
            _reportCategory,
            _reportMessage
        );

        var result = await Client.CreateSubThreadReportAsync(SubThreadName, request);

        if (result.IsSuccess)
        {
            _isReportSuccess = true;
            _isReportPending = false;
        }
        else
        {
            _reportErrorMessage = result.ErrorMessage;
        }

        _isReportBusy = false;
    }

    private void NavigateToUserProfile(string? username)
    {
        if (username is not null)
            Nav.NavigateTo($"/u/{username}");
    }

    private bool IsModeratorDelete => (CurrentUserStore.IsSiteOwner ||
                                       CurrentUserStore.IsSiteAdmin ||
                                       CurrentUserStore.ModeratedSubThreadIds.Contains(SubThreadId) ||
                                       CurrentUserStore.OwnedSubThreadIds.Contains(SubThreadId));

    private bool IsAuthorDelete => Comment.CommentedById == CurrentUserStore.UserId;

    private bool CanEdit => CurrentUserStore.IsLoaded && Comment.CommentedById == CurrentUserStore.UserId;
}