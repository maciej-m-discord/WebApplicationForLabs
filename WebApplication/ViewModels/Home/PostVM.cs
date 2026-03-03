using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace WebApplication.ViewModels.Home;

public class PostVM
{
    public IFormFile? Image { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
}