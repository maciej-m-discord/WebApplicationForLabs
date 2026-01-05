using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using WebApplication.Data.Dtos;
using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public interface IMutualService
{
    Task SendFollowAsync(int senderId, int receiverId);
    Task<Follow> FollowBackAsync(int followId);
    Task UnfollowAsync(int followId, int userId);
    Task<List<UserWithFollowerCountDto>> GetSuggestedFollowsAsync(int userId);
    Task<List<Follow>> GetFollowedUsersAsync(int userId);
    Task<List<Follow>> GetFollowingUsersAsync(int userId);
    Task<List<Mutual>> GetMutualsAsync(int userId);
}