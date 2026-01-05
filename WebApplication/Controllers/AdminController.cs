using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Base;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Services;

namespace WebApplication.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController(IAdminService adminService) : BaseController
{
    private readonly IAdminService _adminService = adminService;

    public IActionResult Index()
    {
        var reportedPosts = _adminService.GetReportedPostsAsync().Result;
        return View(reportedPosts);
    }

    [HttpPost]
    public async Task<IActionResult> ApprovePost(int postId)
    {
        await _adminService.ApproveReportAsync(postId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RejectPost(int postId)
    {
        await _adminService.RejectReportAsync(postId);
        return RedirectToAction("Index");
    }
}