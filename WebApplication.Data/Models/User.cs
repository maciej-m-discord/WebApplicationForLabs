using Microsoft.AspNetCore.Identity;

namespace WebApplication.Data.Models;
using System.ComponentModel.DataAnnotations;

public class User : IdentityUser<int>
{
    
    public string FullName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? BannerImageUrl {get; set;}
    public string? Bio { get; set; }
    //Navigation properties
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Bookmark> Bookmarks = new List<Bookmark>();
    public ICollection<Report> Reports = new List<Report>();
    public ICollection<Story> Stories = new List<Story>();
}