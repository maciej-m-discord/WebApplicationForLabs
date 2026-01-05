namespace WebApplication.Data.Models;
using System.ComponentModel.DataAnnotations;

public class Post 
{
    [Key]
    public int Id { get; set; }
    public string Content { get; set; }
    public string? ImageUrl { get; set; }
    public int NrOfReports { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DateUpdated { get; set; }
    public bool IsPrivate { get; set; }
    
    //Foreign key
    public int UserId { get; set; }
    //Navigation properties
    public User User { get; set; }
    public ICollection<Like>  Likes= new List<Like>();
    public ICollection<Comment> Comments = new List<Comment>();
    public ICollection<Bookmark> Bookmarks = new List<Bookmark>();
    public ICollection<Report> Reports = new List<Report>();
}