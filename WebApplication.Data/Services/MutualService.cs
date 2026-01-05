using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Dtos;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public class MutualService (AppDbContext appDbContext) :IMutualService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    public async Task SendFollowAsync(int senderId, int receiverId)
    {
        var follow = new Follow()
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Status = MutualStatus.Follower,
            CreatedAt = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };
        _appDbContext.Follows.Add(follow);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<Follow> FollowBackAsync(int followId)
    {
        var follow = await _appDbContext.Follows.FirstOrDefaultAsync(f => f.Id == followId);
        if (follow != null)
        {
            follow.Status = MutualStatus.Mutuals;
            follow.DateUpdated = DateTime.UtcNow;
            
            var mutual = new Mutual()
            {
                SenderId = follow.SenderId,
                ReceiverId = follow.ReceiverId,
                DateCreated = DateTime.UtcNow,
                FollowId = follow.Id
            };
            await _appDbContext.Mutuals.AddAsync(mutual);
            await _appDbContext.SaveChangesAsync();
        }

        return follow;
    }

    public async Task UnfollowAsync(int followId, int userId)
    {
        var follow = await _appDbContext.Follows.FirstOrDefaultAsync(f => f.Id == followId);
        if (follow != null)
        {
            if (follow.Status.ToString() == MutualStatus.Mutuals)
            {
                var mutual = await _appDbContext.Mutuals.FirstOrDefaultAsync(m => m.SenderId == follow.SenderId && m.ReceiverId == follow.ReceiverId);
                if (mutual != null)
                {
                    _appDbContext.Mutuals.Remove(mutual);
                }
                follow.Status = MutualStatus.Follower;
                follow.DateUpdated = DateTime.UtcNow;
                if (userId == follow.SenderId)
                {
                    (follow.ReceiverId, follow.SenderId) = (follow.SenderId, follow.ReceiverId);
                }
                _appDbContext.Follows.Update(follow);
            }
            else
            {
                _appDbContext.Follows.Remove(follow);
            }
            await _appDbContext.SaveChangesAsync();
        }
        
    }

    public async Task<List<UserWithFollowerCountDto>> GetSuggestedFollowsAsync(int userId)
    {
        var existingMutual = await _appDbContext.Mutuals
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .ToListAsync();
        
        // following
        
        var followingIds = await _appDbContext.Follows
            .Where(f => (f.SenderId == userId || f.ReceiverId == userId) && f.Status == MutualStatus.Follower)
            .Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
            .ToListAsync();
        
        // get suggested follows excluding existing follows and self
        
        var suggestedFollows = await _appDbContext.Users
            .Where(u => u.Id != userId && !existingMutual.Contains(u.Id) && !followingIds.Contains(u.Id))
            .Select(u => new UserWithFollowerCountDto()
            {
                User = u,
                FollowerCount = _appDbContext.Follows.Count(f => f.ReceiverId == u.Id)
            })
            .Take(5)
            .ToListAsync();
        
        return suggestedFollows;
    }

    public async Task<List<Follow>> GetFollowedUsersAsync(int userId)
    {
        var followsSent = await _appDbContext.Follows
            .Include(f => f.Sender)
            .Include(f => f.Receiver)
            .Where(f => f.SenderId == userId && f.Status == MutualStatus.Follower)
            .ToListAsync();

        return followsSent; 
    }

    public async Task<List<Follow>> GetFollowingUsersAsync(int userId)
    {
        var followsSent = await _appDbContext.Follows
            .Include(f => f.Sender)
            .Include(f => f.Receiver)
            .Where(f => f.ReceiverId == userId && f.Status == MutualStatus.Follower)
            .ToListAsync();

        return followsSent; 
    }

    public async Task<List<Mutual>> GetMutualsAsync(int userId)
    {
        var mutuals = await _appDbContext.Mutuals
            .Include(m=> m.Sender)
            .Include(m=> m.Receiver)
            .Where(m=> m.SenderId == userId || m.ReceiverId == userId)
            .ToListAsync();
        return mutuals;
    }
}