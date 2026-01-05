namespace WebApplication.Data.Models;

public class Story
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ImageUrl { get; set; }
    // Foreign Key
    public int UserId { get; set; }
    // Navigation Property
    public User User { get; set; }
}