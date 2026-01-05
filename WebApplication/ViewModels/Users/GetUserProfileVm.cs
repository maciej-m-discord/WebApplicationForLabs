using WebApplication.Data.Models;

namespace WebApplication.ViewModels.Users;

public class GetUserProfileVm
{
    public User User { get; set; }
    public List<Post> Posts { get; set; }
    public List<Follow> Follows { get; set; }
    public List<Follow> Followers { get; set; }
    public List<Mutual> Mutuals { get; set; }
}