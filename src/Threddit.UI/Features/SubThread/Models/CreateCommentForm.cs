using System.ComponentModel.DataAnnotations;

namespace Threddit.UI.Features.SubThread.Models;

public sealed class CreateCommentForm
{
    [Required, MaxLength(3000)] 
    public string Content { get; set; } = string.Empty;

    [MaxLength(2048)]
    [Display(Name = "image url")]
    public string? ImageUrl { get; set; }
}