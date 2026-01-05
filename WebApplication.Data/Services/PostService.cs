using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebApplication.Data.Dtos;
using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public class PostService(AppDbContext context,INotificationService notificationService):IPostService
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly AppDbContext _context = context;


    public async Task<List<Post>> GetAllPostsAsync(int loggedInUser)
    {
        var allPosts = await _context.Posts
            .Where(p=> (!p.IsPrivate || p.UserId == loggedInUser) && p.Reports.Count < 5)
            .Include(p => p.User)
            .Include(p=> p.Reports)
            .Include( p => p.Likes)
            .Include(p => p.Comments)
            .ThenInclude(c=> c.User)
            .Include(p => p.Bookmarks)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return allPosts;
    }

    public async Task<Post> GetPostByIdAsync(int postId)
    {
        var postDb = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Bookmarks)
            .Include(p => p.Comments).ThenInclude(n => n.User)
            .FirstOrDefaultAsync(p => p.Id == postId);
        return postDb;
    }

    public async Task<List<Post>> GetAllBookmarkedPostsAsync(int loggedInUserId)
    {
        var allBookmarkedPosts = await _context.Bookmarks
            .Include(b => b.Post.Reports)
            .Include(b => b.Post.User)
            .Include(b => b.Post.Comments)
            .ThenInclude(c => c.User)
            .Include(b => b.Post.Likes)
            .Include(b => b.Post.Bookmarks)
            .Where(b => b.UserId == loggedInUserId && 
                        (!b.Post.IsPrivate || b.UserId == loggedInUserId) && 
                        b.Post.Reports.Count < 5)
            .OrderByDescending(f => f.CreatedAt)
            .Select(n => n.Post)
            .ToListAsync();

        return allBookmarkedPosts;
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<Post> RemovePostAsync(int postId)
    {
        var postDb = await _context.Posts.FirstOrDefaultAsync(n => n.Id == postId);
      
        if (postDb != null)
        {
            foreach (var comment in _context.Comments.Where(c=> c.PostId == postId))
            {
                _context.Comments.Remove(comment);
            }
            foreach(var bookmark in _context.Bookmarks.Where(b=> b.PostId == postId))
            {
                _context.Bookmarks.Remove(bookmark);
            }
            foreach(var report in _context.Reports.Where(r=> r.PostId == postId))
            {
                _context.Reports.Remove(report);
            }
            _context.Posts.Remove(postDb);
            await _context.SaveChangesAsync();
        }
        return postDb;
    }

    public async Task AddPostCommentAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }

    public async Task RemovePostCommentAsync(int commentId)
    {
        var commentDb = _context.Comments.FirstOrDefault(c => c.Id == commentId);
        if (commentDb!= null)
        {
            _context.Comments.Remove(commentDb);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<GetNotificationDto> TogglePostLikeAsync(int postId, int userId)
    {
        var response = new GetNotificationDto()
        {
            Success = true,
            SendNotification = false
        };
        
        var like = await _context.Likes
            .Where(l => l.PostId == postId && l.UserId == userId)
            .FirstOrDefaultAsync();
        if (like != null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
        else
        {
            var newLike = new Like()
            {
                PostId = postId,
                UserId = userId
            };
            await _context.Likes.AddAsync(newLike);
            await _context.SaveChangesAsync();
            
            //add notification
            response.SendNotification = true;
        }
        return response;
    }

    public async Task<GetNotificationDto> TogglePostBookmarkAsync(int postId, int userId)
    {
        var response = new GetNotificationDto()
        {
            Success = true,
            SendNotification = false
        };
        
        var bookmark = await _context.Bookmarks
            .Where(l => l.PostId == postId && l.UserId == userId)
            .FirstOrDefaultAsync();
        if (bookmark != null)
        {
            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
        }
        else
        {
            var newBookmark = new Bookmark()
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Bookmarks.AddAsync(newBookmark);
            await _context.SaveChangesAsync();
            
            response.SendNotification = true;
        }

        return response;
    }

    public async Task TogglePostVisibilityAsync(int postId, int userId)
    {
        var post = await _context.Posts
            .FirstOrDefaultAsync(l => l.Id == postId && l.UserId == userId);
        if (post != null)
        {
            post.IsPrivate = !post.IsPrivate;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ReportPostAsync(int postId, int userId)
    {

        var newReport = new Report()
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Reports.AddAsync(newReport);
        await _context.SaveChangesAsync();

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId)!;
        if (post != null)
        {
            post.NrOfReports += 1;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }
}
}