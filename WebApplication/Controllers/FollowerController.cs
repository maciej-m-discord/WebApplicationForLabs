using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Base;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Services;
using WebApplication.ViewModels.Mutuals;

namespace WebApplication.Controllers;
[Authorize]
public class FollowerController(IMutualService mutualService, INotificationService notificationService) : BaseController
{
    private readonly IMutualService _mutualService = mutualService;
    private readonly INotificationService _notificationService = notificationService;
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToLogin();
        var followedData = new MutualVm
        {
            FollowsSent = await _mutualService.GetFollowedUsersAsync(userId.Value),
            FollowsReceived = await _mutualService.GetFollowingUsersAsync(userId.Value),
            Mutuals = await _mutualService.GetMutualsAsync(userId.Value)
        };
        
        return View(followedData);
    }

    [HttpPost]
    public async Task<IActionResult> Follow(int receiverId)
    {
        var userId = GetUserId();
        var userName = GetUserFullName();
        var userProfileImageUrl = GetUserProfileImageUrl();
        if (!userId.HasValue) return RedirectToLogin();
        
        await _mutualService.SendFollowAsync(userId.Value, receiverId);

        await _notificationService.AddNewNotificationAsync(receiverId, userId.Value, NotificationType.Follow, userName,
            null, userProfileImageUrl);
            
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Unfollow(int followId)
    {
        await _mutualService.UnfollowAsync(followId, GetUserId().Value);
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public async Task<IActionResult> FollowBack(int followId)
    {
        var userId = GetUserId();
        var userName = GetUserFullName();
        var userProfileImageUrl = GetUserProfileImageUrl();
        if (!userId.HasValue) return RedirectToLogin();
        
        var follow = await _mutualService.FollowBackAsync(followId);
        
        await _notificationService.AddNewNotificationAsync(follow.SenderId, userId.Value, NotificationType.Follow, userName,
            null, userProfileImageUrl);
        
        return RedirectToAction("Index");
    }
}