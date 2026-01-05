using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication.Controllers.Base;
using WebApplication.Data.Services;
using WebApplication.Hubs;

namespace WebApplication.Controllers;
[Authorize]
public class NotificationController(
    INotificationService notificationService,
    IHubContext<NotificationHub> notificationHub) : BaseController
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetCount()
    {

        var userId = GetUserId();
        if (!userId.HasValue) RedirectToLogin();

        var count = await _notificationService.GetUnreadNotificationCountAsync(userId.Value);
        return Json(count);
    }

    public async Task<IActionResult> GetNotifications()
    {
        var userId = GetUserId();
        if (!userId.HasValue) RedirectToLogin();

        var notifications = await _notificationService.GetNotificationsAsync(userId.Value);
        return PartialView("Notifications/_Notifications", notifications);
    }

    [HttpPost]
    public async Task<IActionResult> SetAsRead(int notificationId)
    {
        var userId = GetUserId();
        if (!userId.HasValue) RedirectToLogin();
        else
        {
            await _notificationService.SetNotificationsAsReadAsync(notificationId);
            var notifications = await _notificationService.GetNotificationsAsync(userId.Value);
            return PartialView("Notifications/_Notifications", notifications);
        }

        return RedirectToAction("Index", "Home");
    }

}