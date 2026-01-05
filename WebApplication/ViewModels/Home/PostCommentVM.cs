using System.ComponentModel.DataAnnotations;

namespace WebApplication.ViewModels.Home;

public class PostCommentVM
{
    public int PostId { get; set; }
    public string Content { get; set; } = string.Empty;
}