namespace WebApplication.Data.Models;

public class Follow
{
    public int Id { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime DateUpdated { get; set; }
    public int SenderId { get; set; }
    public User Sender { get; set; }
    public int ReceiverId { get; set; }
    public User Receiver { get; set; }
}