using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Threddit.Contracts.Requests.Posts;
using Threddit.UI.ApiClient;
using Threddit.UI.Features.SubThread.Models;

namespace Threddit.UI.Features.SubThread;

public sealed partial class CreatePost : ComponentBase
{
    [Parameter] public string SubThreadName { get; set; } = string.Empty;

    [Inject] private ThredditApiClient Client { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private readonly CreatePostForm _form = new();
    private bool _isBusy;
    private bool _isImageUploadBusy;
    private string? _errorMessage;
    private string? _imageUploadErrorMessage;

    private IBrowserFile? _pendingImageFile;
    private string? _imagePreviewUrl;

    private async Task OnPostImageSelectedAsync(InputFileChangeEventArgs e)
    {
        _pendingImageFile = e.File;
        _imageUploadErrorMessage = null;

        var buffer = new byte[_pendingImageFile.Size];
        await _pendingImageFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).ReadExactlyAsync(buffer);
        _imagePreviewUrl = $"data:{_pendingImageFile.ContentType};base64,{Convert.ToBase64String(buffer)}";
    }

    private async Task HandleCreateAsync()
    {
        _isBusy = true;
        _errorMessage = null;

        if (_pendingImageFile is not null)
        {
            _isImageUploadBusy = true;
            StateHasChanged();

            await using var stream = _pendingImageFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            var uploadResult = await Client
                .UploadImageAsync(stream, _pendingImageFile.Name, _pendingImageFile.ContentType, "post");
            _isImageUploadBusy = false;

            if (uploadResult.IsSuccess)
                _form.ImageUrl = uploadResult.Value!;
            else
            {
                _imageUploadErrorMessage = uploadResult.ErrorMessage ?? "Upload failed.";
                _isBusy = false;
                return;
            }
        }

        var request = new CreatePostApiRequest(_form.Title, _form.Content ?? string.Empty, _form.ImageUrl);
        var result = await Client.CreatePostAsync(SubThreadName, request);

        if (result.IsSuccess)
        {
            _pendingImageFile = null;
            _imagePreviewUrl = null;
            var post = result.Value!;
            Nav.NavigateTo($"/t/{SubThreadName}/posts/{post.Id}/{post.Slug}");
        }
        else
        {
            _errorMessage = result.ErrorMessage;
            _isBusy = false;
        }
    }

    private void NavigateToSubThread()
    {
        Nav.NavigateTo($"/t/{SubThreadName}");
    }
}