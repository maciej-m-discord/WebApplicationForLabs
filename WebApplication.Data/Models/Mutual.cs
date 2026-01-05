namespace WebApplication.Data.Models;

public class Mutual
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public int SenderId { get; set; }
    public virtual User Sender { get; set; }
    public int ReceiverId { get; set; }
    public virtual User Receiver { get; set; }
    public int FollowId { get; set; }
}