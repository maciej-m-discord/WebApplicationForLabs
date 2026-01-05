using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Base;
using WebApplication.Data.Models;
using WebApplication.Data.Services;
using WebApplication.ViewModels.Users;

namespace WebApplication.Controllers;
[Authorize]
public class UserController(IUserService userService, UserManager<User> userManager) : BaseController
{
    private readonly IUserService _userService = userService;
    private readonly UserManager<User> _userManager = userManager;
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Details(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var userPosts = await _userService.GetUserPosts(userId);
        var userFollows = await _userService.GetUserFollows(userId);
        var userFollowers = await _userService.GetUserFollowers(userId);
        var userMutuals = await _userService.GetUserMutuals(userId);

        var userProfileVm = new GetUserProfileVm()
        {
            User = user,
            Posts = userPosts,
            Follows = userFollows,
            Followers = userFollowers,
            Mutuals = userMutuals
        };
        return View(userProfileVm);
    }
}