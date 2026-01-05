using WebApplication.Data.Models;

namespace WebApplication.ViewModels.Mutuals;

public class MutualVm
{
    public List<Follow> FollowsSent = new List<Follow>();
    public List<Follow> FollowsReceived = new List<Follow>();
    public List<Mutual> Mutuals = new List<Mutual>();
}