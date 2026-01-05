using Microsoft.AspNetCore.Http;
using WebApplication.Data.Dtos;
using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public interface IPostService
{
    Task<List<Post>> GetAllPostsAsync(int loggedInUserId);
    Task<Post> GetPostByIdAsync(int postId);
    Task<List<Post>> GetAllBookmarkedPostsAsync(int loggedInUserId);
    Task<Post> CreatePostAsync(Post post);
    Task<Post> RemovePostAsync(int postId);
    Task AddPostCommentAsync(Comment comment);
    Task RemovePostCommentAsync(int commentId);
    Task<GetNotificationDto> TogglePostLikeAsync(int postId, int userId);
    Task<GetNotificationDto> TogglePostBookmarkAsync(int postId, int userId);
    Task TogglePostVisibilityAsync(int postId, int userId);
    Task ReportPostAsync(int postId, int userId);
    
}