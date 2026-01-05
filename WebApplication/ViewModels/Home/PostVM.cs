using System.ComponentModel.DataAnnotations;

namespace WebApplication.ViewModels.Home;

public class PostVM
{
    [Required]
    public string Content { get; set; } = string.Empty;
    public IFormFile Image { get; set; }
}