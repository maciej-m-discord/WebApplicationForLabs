using System.ComponentModel.DataAnnotations;

namespace WebApplication.ViewModels.Settings;

public class UpdateProfileVM
{
    [StringLength(40, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters")]
    public string FullName { get; set; }
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 40 characters")]
    [RegularExpression(@"^[a-z0-9_.-]+$", ErrorMessage = "Username can only contain lowercase letters, numbers, underscores, dots, and hyphens")]
    public string UserName { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?'-]+$", ErrorMessage = "Bio contains invalid characters")]
    public string Bio { get; set; }
}