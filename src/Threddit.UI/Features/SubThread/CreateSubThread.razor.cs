using Microsoft.AspNetCore.Components;
using Threddit.Contracts.Requests.SubThreads;
using Threddit.UI.ApiClient;
using Threddit.UI.Features.SubThread.Models;

namespace Threddit.UI.Features.SubThread;

public sealed partial class CreateSubThread : ComponentBase
{
    [Inject] ThredditApiClient Client { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    private readonly CreateSubThreadForm _form = new();
    private string? _errorMessage;
    private bool _busy;

    private async Task HandleCreateAsync()
    {
        _busy = true;
        _errorMessage = null;

        var request = new CreateSubThreadApiRequest(_form.Name, _form.Description, _form.BannerImageUrl);
        var result = await Client.CreateSubThreadAsync(request);

        if (result.IsSuccess)
        {
            Nav.NavigateTo($"/t/{result.Value!.Name}");
        }
        else
        {
            _errorMessage = result.ErrorMessage;
        }

        _busy = false;
    }
}