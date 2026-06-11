using System.ComponentModel.DataAnnotations;

namespace Threddit.UI.Features.SubThread.Models;

public sealed class CreatePostForm
{
    [Required, MaxLength(100)]
    [Display(Name = "title")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(3000)]
    public string? Content { get; set; }
    
    [MaxLength(2048)]
    [Display(Name = "image url")]
    public string? ImageUrl { get; set; }
}