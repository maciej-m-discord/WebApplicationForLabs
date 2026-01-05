using System.Runtime.InteropServices.JavaScript;

namespace WebApplication.Data.Models;

public class Notification
{
    public int Id { get; set; }
    public int ReceiverId { get; set; }
    public int SenderId { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public string Type { get; set; }
    public int? PostId { get; set; }
    public string? UserProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DateUpdated { get; set; }
    
}