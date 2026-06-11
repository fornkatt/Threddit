using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Threddit.Contracts.Requests.Comments;
using Threddit.Contracts.Requests.Posts;
using Threddit.Contracts.Requests.Reports;
using Threddit.Contracts.Responses.Comments;
using Threddit.Contracts.Responses.Posts;
using Threddit.UI.ApiClient;
using Threddit.UI.Features.SubThread.Models;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Features.SubThread;

public sealed partial class ViewPost : ComponentBase
{
    [Parameter] public string SubThreadName { get; set; } = string.Empty;
    [Parameter] public Guid PostId { get; set; }
    [Parameter] public string PostSlug { get; set; } = string.Empty;

    [Inject] private ThredditApiClient Client { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private ICurrentUserStore CurrentUserStore { get; set; } = null!;

    private GetPostWithCommentsApiResponse? _post;
    private List<GetCommentApiResponse> _comments = [];

    private bool _isLoading;
    private string? _errorMessage;

    private bool _isVoteBusy;

    private readonly CreateCommentForm _commentForm = new();
    private bool _isCommentBusy;
    private bool _isCommentImageUploadBusy;
    private string? _commentImageUploadErrorMessage;
    private string? _commentErrorMessage;

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

    private IBrowserFile? _pendingImageFile;
    private string? _imagePreviewUrl;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;

        var result = await Client.GetPostWithCommentsAsync(SubThreadName, PostId);

        if (result.IsSuccess)
        {
            _post = result.Value!;
            _comments = [.._post.Comments];
        }
        else
        {
            _errorMessage = result.ErrorMessage;
        }

        _isLoading = false;
    }

    private async Task OnCommentImageSelectedAsync(InputFileChangeEventArgs e)
    {
        _pendingImageFile = e.File;
        _commentImageUploadErrorMessage = null;

        var buffer = new byte[_pendingImageFile.Size];
        await _pendingImageFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).ReadExactlyAsync(buffer);
        _imagePreviewUrl = $"data:{_pendingImageFile.ContentType};base64,{Convert.ToBase64String(buffer)}";
    }

    private async Task VotePostAsync(bool isUpvote)
    {
        if (_isVoteBusy || _post is null)
            return;

        _isVoteBusy = true;

        var result = await Client.VotePostAsync(SubThreadName, _post.Id, isUpvote);
        if (result.IsSuccess)
            _post = _post with { Score = result.Value };

        _isVoteBusy = false;
    }

    private async Task HandleCommentAsync()
    {
        if (_post is null) return;

        _isCommentBusy = true;
        _commentErrorMessage = null;

        if (_pendingImageFile is not null)
        {
            _isCommentImageUploadBusy = true;
            StateHasChanged();
            
            await using var stream = _pendingImageFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            var uploadResult = await Client
                .UploadImageAsync(stream, _pendingImageFile.Name, _pendingImageFile.ContentType, "comment");
            _isCommentImageUploadBusy = false;
            
            if (uploadResult.IsSuccess)
                _commentForm.ImageUrl = uploadResult.Value!;
            else
            {
                _commentImageUploadErrorMessage = uploadResult.ErrorMessage ?? "Upload failed.";
                _isCommentBusy = false;
                return;
            }
        }

        var request = new CreateCommentApiRequest(_commentForm.Content, ImageUrl: _commentForm.ImageUrl);
        var result = await Client.CreateCommentAsync(SubThreadName, _post.Id, request);

        if (result.IsSuccess)
        {
            var c = result.Value!;
            _comments.Insert(0, new GetCommentApiResponse(
                c.Id,
                c.CommentedById,
                c.CommentedByUsername,
                c.CommentedByProfilePicture,
                c.Content,
                c.ImageUrl,
                c.Score,
                c.CommentedAt,
                null,
                c.IsDeleted,
                [],
                c.HasReplies
            ));
            _commentForm.Content = string.Empty;
            _commentForm.ImageUrl = null;
            _pendingImageFile = null;
            _imagePreviewUrl = null;
        }
        else
        {
            _commentErrorMessage = result.ErrorMessage;
        }

        _isCommentBusy = false;
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
        if (_post is null)
            return;

        _isDeleteBusy = true;
        _deleteErrorMessage = null;

        var reason = (IsModeratorDelete && !IsAuthorDelete) ? _deleteReason : null;
        var result = await Client.DeletePostAsync(SubThreadName, _post.Id, reason);

        if (result.IsSuccess)
        {
            NavigateToSubThread();
            return;
        }

        _deleteErrorMessage = result.ErrorMessage;
        _isDeleteBusy = false;
    }

    private void RequestEdit()
    {
        _isEditPending = true;
        _editContent = _post?.Content ?? string.Empty;
        _editErrorMessage = null;
    }

    private void CancelEdit()
    {
        _isEditPending = false;
        _editContent = string.Empty;
    }

    private async Task ConfirmEditAsync()
    {
        if (_post is null)
            return;

        _isEditBusy = true;
        _editErrorMessage = null;

        var result = await Client.EditPostAsync(SubThreadName, _post.Id,
            new EditPostApiRequest(_editContent, _post.ImageUrl));

        if (result.IsSuccess)
        {
            _post = _post with { Content = _editContent };
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

    private void CancelReportPost() => _isReportPending = false;

    private async Task ConfirmReportAsync()
    {
        _isReportBusy = true;
        _reportErrorMessage = null;

        var request = new CreateSubThreadReportApiRequest(
            "Post",
            _post!.Id,
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

    private bool IsModeratorDelete => _post is not null &&
                                      (CurrentUserStore.IsSiteOwner ||
                                       CurrentUserStore.IsSiteAdmin ||
                                       CurrentUserStore.ModeratedSubThreadIds.Contains(_post.SubThreadId) ||
                                       CurrentUserStore.OwnedSubThreadIds.Contains(_post.SubThreadId));

    private bool IsAuthorDelete => _post?.PostedById == CurrentUserStore.UserId;

    private bool CanEdit => CurrentUserStore.IsLoaded && _post?.PostedById == CurrentUserStore.UserId;

    private void NavigateToSubThread() => Nav.NavigateTo($"/t/{SubThreadName}");
}