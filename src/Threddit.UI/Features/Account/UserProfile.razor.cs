using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Threddit.Contracts.Requests.Users;
using Threddit.Contracts.Responses.Users;
using Threddit.UI.ApiClient;
using Threddit.UI.Interfaces;

namespace Threddit.UI.Features.Account;

public sealed partial class UserProfile : ComponentBase
{
    [Inject] private ThredditApiClient Client { get; set; } = null!;
    [Inject] private ICurrentUserStore CurrentUserStore { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    [Parameter] public string Username { get; set; } = string.Empty;

    private GetUserProfileApiResponse? _profile;
    private bool _isLoading;
    private string? _errorMessage;

    private bool _isEditing;
    private bool _isEditBusy;
    private bool _isUploadBusy;
    private string? _editErrorMessage;
    private string? _uploadErrorMessage;
    private string _editProfilePicture = string.Empty;
    private string _editDescription = string.Empty;

    private IBrowserFile? _pendingImageFile;
    private string? _imagePreviewUrl;

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        _errorMessage = null;

        var result = await Client.GetUserProfileAsync(Username);

        if (result.IsSuccess)
            _profile = result.Value!;
        else
            _errorMessage = result.ErrorMessage ?? "Failed to load user profile.";

        _isLoading = false;
    }

    private async Task OnProfilePictureSelectedAsync(InputFileChangeEventArgs e)
    {
        _pendingImageFile = e.File;
        _uploadErrorMessage = null;

        var buffer = new byte[_pendingImageFile.Size];
        await _pendingImageFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024).ReadExactlyAsync(buffer);
        _imagePreviewUrl = $"data:{_pendingImageFile.ContentType};base64,{Convert.ToBase64String(buffer)}";
    }

    private void StartEdit()
    {
        _editProfilePicture = _profile?.ProfilePicture ?? string.Empty;
        _editDescription = _profile?.Description ?? string.Empty;
        _isEditing = true;
        _editErrorMessage = null;
    }

    private void CancelEdit()
    {
        _isEditing = false;
        _pendingImageFile = null;
        _imagePreviewUrl = null;
        _editProfilePicture = string.Empty;
        _editDescription = string.Empty;
    }

    private async Task SaveEditAsync()
    {
        _isEditBusy = true;
        _editErrorMessage = null;

        if (_pendingImageFile is not null)
        {
            _isUploadBusy = true;
            StateHasChanged();

            await using var stream = _pendingImageFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
            var uploadResult = await Client
                .UploadImageAsync(stream, _pendingImageFile.Name, _pendingImageFile.ContentType, "profile");
            _isUploadBusy = false;

            if (uploadResult.IsSuccess)
                _editProfilePicture = uploadResult.Value!;
            else
            {
                _uploadErrorMessage = uploadResult.ErrorMessage ?? "Upload failed.";
                _isEditBusy = false;
                return;
            }
        }

        var request = new EditUserProfileApiRequest(
            string.IsNullOrWhiteSpace(_editProfilePicture) ? null : _editProfilePicture,
            string.IsNullOrWhiteSpace(_editDescription) ? null : _editDescription);

        var result = await Client.EditUserProfileAsync(request);

        if (result.IsSuccess)
        {
            _isEditing = false;
            var reload = await Client.GetUserProfileAsync(Username);

            if (reload.IsSuccess)
                _profile = reload.Value;
            
            _pendingImageFile = null;
            _imagePreviewUrl = null;
            _editProfilePicture = string.Empty;
            _editDescription = string.Empty;
        }
        else
        {
            _editErrorMessage = result.ErrorMessage;
        }

        _isEditBusy = false;
    }

    private void InitiateConversation() =>
        Nav.NavigateTo($"/conversations/new?with={Uri.EscapeDataString(Username)}");
}