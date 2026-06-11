using System.ComponentModel.DataAnnotations;

namespace Threddit.UI.Features.SubThread.Models;

public sealed class CreateSubThreadForm
{
    [Required, MaxLength(50)] 
    [Display(Name = "SubThread name")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [MaxLength(2048)]
    public string? BannerImageUrl { get; set; }
}