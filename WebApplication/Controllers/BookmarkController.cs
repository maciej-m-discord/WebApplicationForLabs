using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Base;
using WebApplication.Data.Services;

namespace WebApplication.Controllers;
[Authorize]
public class BookmarkController(IPostService postService) : BaseController
{
    private readonly IPostService _postService = postService;
    // GET
    public async Task<IActionResult> Index()
    {
        int? loggedInUserId = GetUserId();
        if(loggedInUserId == null) return RedirectToLogin();
        var bookmarkedPosts = await _postService.GetAllBookmarkedPostsAsync(loggedInUserId.Value);
        
        return View(bookmarkedPosts);
    }
}