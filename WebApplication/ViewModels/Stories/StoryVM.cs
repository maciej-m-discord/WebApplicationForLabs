using System.ComponentModel.DataAnnotations;

namespace WebApplication.ViewModels.Stories;

public class StoryVM
{
    [Required]
    public IFormFile Image { get; set; }
}