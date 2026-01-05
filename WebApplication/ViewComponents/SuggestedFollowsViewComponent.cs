using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using WebApplication.Data.Dtos;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Services;
using WebApplication.ViewModels.Mutuals;

namespace WebApplication.ViewComponents;

public class SuggestedFollowsViewComponent (IMutualService mutualService) :ViewComponent
{
    private readonly IMutualService _mutualService = mutualService;
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var loggedInUserId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = int.Parse(loggedInUserId);
        var suggestedFollows = await _mutualService.GetSuggestedFollowsAsync(userId);
        var suggestedFollowsVm = suggestedFollows.Select(sf => new UserWithFollowerCountVm()
        {
            UserId = sf.User.Id,
            FullName = sf.User.FullName,
            ProfilePictureUrl = sf.User.ProfilePictureUrl,
            FollowerCount = sf.FollowerCount
        }).ToList();
        return View(suggestedFollowsVm);
    }
}