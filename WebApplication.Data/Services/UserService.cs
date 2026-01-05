using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public class UserService(AppDbContext appDbContext) : IUserService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    
    public async Task<User> GetUser(int loggedInUserId)
    {
        return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == loggedInUserId) ?? new User();
    }

    public async Task UpdateUserProfilePicture(int loggedInUserId, string profilePictureUrl)
    {
        var userDb= await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == loggedInUserId);
        if (userDb != null)
        {
            userDb.ProfilePictureUrl = profilePictureUrl;
            _appDbContext.Users.Update(userDb);
            await _appDbContext.SaveChangesAsync();
        }
    }

    public async Task UpdateUserCoverPicture(int loggedInUserId, string coverPictureUrl)
    {
        var userDb = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == loggedInUserId);
        if (userDb != null)
        {
            userDb.BannerImageUrl = coverPictureUrl;
            _appDbContext.Users.Update(userDb);
            await _appDbContext.SaveChangesAsync();
        }
    }

    public async Task<List<Post>> GetUserPosts(int userId)
    {
        var allPosts = await _appDbContext.Posts
            .Where(p=> p.UserId == userId && p.Reports.Count < 5)
            .Include(p => p.User)
            .Include(p=> p.Reports)
            .Include( p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.Bookmarks)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return allPosts;
    }

    public async Task<List<Follow>> GetUserFollows(int userId)
    {
        var follows = await _appDbContext.Follows
            .Where(f => f.SenderId == userId
                        || (f.ReceiverId == userId && _appDbContext.Mutuals.Any(m =>
                            (m.SenderId == f.SenderId && m.ReceiverId == userId)
                            || (m.SenderId == userId && m.ReceiverId == f.SenderId))))
            .Include(f => f.Receiver)
            .Include(f => f.Sender)
            .ToListAsync();
        
        return follows;
    }

    public async Task<List<Follow>> GetUserFollowers(int userId)
    {
        var followers = await _appDbContext.Follows
            .Where(f => f.ReceiverId == userId
                        || (f.SenderId == userId && _appDbContext.Mutuals.Any(m =>
                            (m.SenderId == f.ReceiverId && m.ReceiverId == userId)
                            || (m.SenderId == userId && m.ReceiverId == f.ReceiverId))))
            .Include(f => f.Sender)
            .Include(f => f.Receiver)
            .ToListAsync();
        return followers;
    }

    public async Task<List<Mutual>> GetUserMutuals(int userId)
    {
        var mutuals = await _appDbContext.Mutuals
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .ToListAsync();
        return mutuals;
    }
}