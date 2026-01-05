namespace WebApplication.Data.Models;

public class Hashtag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DateUpdated { get; set; }
}