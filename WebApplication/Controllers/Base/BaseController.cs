using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data.Helpers.Constants;

namespace WebApplication.Controllers.Base;

public abstract class BaseController : Controller
{
    protected int? GetUserId()
    {
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(loggedInUserId))
            return null;
        return int.Parse(loggedInUserId);
    }

    protected string? GetUserFullName()
    {
        return User.FindFirstValue(ClaimTypes.Name);
    }
    protected IActionResult RedirectToLogin()
    {
        return RedirectToAction("Login", "Authentication");
    }
    
    protected string? GetUserProfileImageUrl()
    {
        return User.FindFirstValue(CustomClaim.ImageUrl);
    }
}