using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public class AdminService(AppDbContext appDbContext) :IAdminService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    
    public async Task<List<Post>> GetReportedPostsAsync()
    {
        var posts = await _appDbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Reports)
            .Where(p => p.NrOfReports > 0)
            .ToListAsync();
        return posts;
    }

    public async Task ApproveReportAsync(int postId)
    {
        var post = await _appDbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);

        if (post != null)
        {
            foreach (var comment in _appDbContext.Comments.Where(c=> c.PostId == postId))
            {
                _appDbContext.Comments.Remove(comment);
            }
            foreach(var bookmark in _appDbContext.Bookmarks.Where(b=> b.PostId == postId))
            {
                _appDbContext.Bookmarks.Remove(bookmark);
            }
            foreach(var report in _appDbContext.Reports.Where(r=> r.PostId == postId))
            {
                _appDbContext.Reports.Remove(report);
            }
            _appDbContext.Posts.Remove(post);
            await _appDbContext.SaveChangesAsync();
        }
            
    }

    public async Task RejectReportAsync(int postId)
    {
        var post = await _appDbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post != null)
        {
            var reports = _appDbContext.Reports.Where(r => r.PostId == postId);
            _appDbContext.Reports.RemoveRange(reports);
            post.NrOfReports = 0;
            _appDbContext.Posts.Update(post);
            await _appDbContext.SaveChangesAsync();
        }
    }
}