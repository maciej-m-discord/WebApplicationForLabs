namespace WebApplication.Data.Models;

public class Report
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    
    //Navigation properties
    public Post Post { get; set; }
    public User User { get; set; }
}