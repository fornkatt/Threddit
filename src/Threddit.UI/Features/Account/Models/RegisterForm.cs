using System.ComponentModel.DataAnnotations;

namespace Threddit.UI.Features.Account.Models;

public sealed class RegisterForm
{
    [Required, MaxLength(40)]
    [Display(Name = "username")]
    public string Username { get; set; } = string.Empty;
    
    [Required, EmailAddress, MaxLength(150)]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "email")]
    public string Email { get; set; } = string.Empty;
    
    [Required, MinLength(8)]
    [DataType(DataType.Password)]
    [Display(Name = "password")]
    public string Password { get; set; } = string.Empty;
}