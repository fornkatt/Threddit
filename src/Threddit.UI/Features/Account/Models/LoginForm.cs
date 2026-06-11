using System.ComponentModel.DataAnnotations;

namespace Threddit.UI.Features.Account.Models;

public sealed class LoginForm
{
    [Required, Display(Name = "username or email")]
    public string UsernameOrEmail { get; set; } = string.Empty;
    
    [Required, Display(Name = "password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}