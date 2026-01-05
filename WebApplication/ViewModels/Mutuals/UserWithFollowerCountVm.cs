namespace WebApplication.ViewModels.Mutuals;

public class UserWithFollowerCountVm
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int FollowerCount { get; set; }
    public string FollowerCountDisplay => FollowerCount == 0 ? "No followers" : FollowerCount == 1 ? "1 follower" : $"{FollowerCount} followers";
}