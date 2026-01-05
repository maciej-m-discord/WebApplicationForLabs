using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public interface IUserService
{
    Task<User> GetUser(int loggedInUserId); 
    Task UpdateUserProfilePicture(int loggedInUserId, string profilePictureUrl);
    Task UpdateUserCoverPicture(int loggedInUserId, string coverPictureUrl);
    Task<List<Post>> GetUserPosts(int userId);
    Task<List<Follow>> GetUserFollows(int userId);
    Task<List<Follow>> GetUserFollowers(int userId);
    Task<List<Mutual>> GetUserMutuals(int userId);
}