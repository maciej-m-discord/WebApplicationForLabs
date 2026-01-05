using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public interface INotificationService
{
    Task AddNewNotificationAsync(int receiverId,int senderId, string type, string userFullName, int? postId, string? userProfileImageUrl);
    Task<int> GetUnreadNotificationCountAsync(int userId);
    Task<List<Notification>> GetNotificationsAsync(int userId);
    Task SetNotificationsAsReadAsync(int notificationId);
}