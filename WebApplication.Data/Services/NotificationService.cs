using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Models;
using WebApplication.Hubs;

namespace WebApplication.Data.Services;

public class NotificationService(AppDbContext appDbContext, IHubContext<NotificationHub> hubContext): INotificationService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IHubContext<NotificationHub> _hubContext = hubContext;
    public async Task AddNewNotificationAsync(int receiverId, int senderId, string type, string userFullName,
        int? postId, string? userProfileImageUrl)
    {
        var newNotification = new Notification
        {
            ReceiverId = receiverId,
            SenderId =  senderId,
            Message = GetPostMessage(type, userFullName),
            Type = type,
            IsRead = false,
            PostId = postId,
            UserProfileImageUrl = userProfileImageUrl,
            CreatedAt = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };
        
        await _appDbContext.Notifications.AddAsync(newNotification);
        await _appDbContext.SaveChangesAsync();
        
        var notificationNumber = await GetUnreadNotificationCountAsync(receiverId);
        
        await _hubContext.Clients.User(receiverId.ToString())
            .SendAsync("ReceiveNotification", notificationNumber);
    }

    public async Task<int> GetUnreadNotificationCountAsync(int userId)
    {
        var count = await _appDbContext.Notifications.Where(n => n.ReceiverId == userId && !n.IsRead).CountAsync();

        return count;
    }

    public async Task<List<Notification>> GetNotificationsAsync(int userId)
    {
        var allNotifications = await _appDbContext.Notifications
            .Where(n => n.ReceiverId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return allNotifications;
    }

    public async Task SetNotificationsAsReadAsync(int notificationId)
    {
        var notificationDb = await _appDbContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

        if (notificationDb != null)
        {
            notificationDb.DateUpdated = DateTime.UtcNow;
            notificationDb.IsRead = true;
            
            _appDbContext.Notifications.Update(notificationDb);
            await _appDbContext.SaveChangesAsync();
        }
        
    }

    private string GetPostMessage(string type, string userFullName)
    {
        var message = string.Empty;
        switch (type)
        {
            case NotificationType.Like:
                message = $"{userFullName} liked your post.";
                return message;
            case NotificationType.Comment:
                message = $"{userFullName} commented on your post.";
                return message;
            case NotificationType.Bookmark:
                message = $"{userFullName} bookmarked your post.";
                return message;
            case NotificationType.Follow:
                message = $"{userFullName} followed you.";
                return message;
            default:
                return message;
        }
       
    }
}