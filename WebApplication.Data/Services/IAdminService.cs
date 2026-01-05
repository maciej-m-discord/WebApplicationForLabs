using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public interface IAdminService
{
    Task<List<Post>> GetReportedPostsAsync();
    Task ApproveReportAsync(int postId);
    Task RejectReportAsync(int postId);
}