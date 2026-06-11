using Microsoft.AspNetCore.Components;

namespace Threddit.UI.Features.Shared;

public sealed partial class UserAvatarComponent : ComponentBase
{
    [Parameter] public string? Username { get; set; }
    [Parameter] public string? ProfilePictureUrl { get; set; }
    [Parameter] public string? CssClass { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }
}