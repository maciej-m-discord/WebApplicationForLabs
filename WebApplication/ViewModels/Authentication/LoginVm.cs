using System.ComponentModel.DataAnnotations;

namespace WebApplication.ViewModels.Authentication;

public class LoginVm
{
    [Required(ErrorMessage =  "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    [Required(ErrorMessage =  "Password is required")]
    public string Password { get; set; }
}