using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using WebApplication.Controllers.Base;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Helpers.Enums;
using WebApplication.Data.Models;
using WebApplication.Data.Services;
using WebApplication.Hubs;
using WebApplication.ViewModels.Home;

namespace WebApplication.Controllers;
[Authorize]
public class HomeController(ILogger<HomeController> logger, IPostService postService, IHashtagService hashtagService, IFileService fileService, INotificationService notificationService)
    : BaseController
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IHashtagService _hashtagService = hashtagService;
    private readonly IFileService _fileService = fileService;
    private readonly IPostService _postService = postService;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<IActionResult> Index()
    {
        int? userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        var posts = await _postService.GetAllPostsAsync(userId.Value);
        return View(posts);
    }
    
    public async Task<IActionResult> Details(int postId)
    {
        var post = await _postService.GetPostByIdAsync(postId);
        return View(post);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost(PostVM post)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Home", post);
        }
        int? userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        var imageUploadPath = await _fileService.UploadImageAsync(post.Image, ImageFileType.PostImage);

        if (string.IsNullOrEmpty(post.Content))
        {
            post.Content = " ";
        }
        
        //Create a new post
        var newPost = new Post
        {
            Content = post.Content,
            CreatedAt = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            ImageUrl = imageUploadPath,
            NrOfReports = 0,
            UserId = userId.Value
        };
        
        // Check and save the image
        await _postService.CreatePostAsync(newPost);
        await _hashtagService.ProcessHashtags(post.Content);
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVm)
    {
        int? userId = GetUserId();
        var userFullName = GetUserFullName();
        var userProfileImageUrl = GetUserProfileImageUrl();
        if(userId == null) return RedirectToLogin();
        
        var result = await _postService.TogglePostLikeAsync(postLikeVm.PostId, userId.Value);
        
        var post= await _postService.GetPostByIdAsync(postLikeVm.PostId);
        
        if (result.SendNotification && userId != post.UserId)
            await _notificationService.AddNewNotificationAsync(post.UserId, userId.Value, NotificationType.Like, userFullName ?? string.Empty, postLikeVm.PostId, userProfileImageUrl);
        
        return PartialView("Home/_Posts", post);
    }
    
    [HttpPost]
    public async Task<IActionResult> TogglePostBookmark(PostBookmarkVM postBookmarkVm)
    {
        int? userId = GetUserId();
        var userFullName = GetUserFullName();
        var userProfileImageUrl = GetUserProfileImageUrl();
        if(userId == null) return RedirectToLogin();
        var result = await _postService.TogglePostBookmarkAsync(postBookmarkVm.PostId, userId.Value);
        
        var post= await _postService.GetPostByIdAsync(postBookmarkVm.PostId);
        
        if (result.SendNotification && userId != post.UserId)
            await _notificationService.AddNewNotificationAsync(post.UserId, userId.Value, NotificationType.Bookmark, userFullName ?? string.Empty, postBookmarkVm.PostId, userProfileImageUrl);
        
        return PartialView("Home/_Posts", post);
    }
    [HttpPost]
    public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVm)
    {
        int? userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        await _postService.TogglePostVisibilityAsync(postVisibilityVm.PostId, userId.Value);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVm)
    {
        // if (!ModelState.IsValid)
        // {
        //     TempData["ErrorMessage"] = "Invalid post data. Please try again.";
        //     return RedirectToAction("Index");
        // }
        int? userId = GetUserId();
        var userFullName = GetUserFullName();
        var userProfileImageUrl = GetUserProfileImageUrl();
        if(userId == null) return RedirectToLogin();
        
        
        if (string.IsNullOrEmpty(postCommentVm.Content))
        {
            postCommentVm.Content = " ";
        }
        
        var newComment = new Comment()
        {
            PostId = postCommentVm.PostId,
            UserId = userId.Value,
            Content = postCommentVm.Content,
            CreatedAt = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };
        await _postService.AddPostCommentAsync(newComment);
        
        var post= await _postService.GetPostByIdAsync(postCommentVm.PostId);
        
        if(userId != post.UserId)
            await _notificationService.AddNewNotificationAsync(post.UserId, userId.Value, NotificationType.Comment, userFullName ?? string.Empty, postCommentVm.PostId, userProfileImageUrl);
        
        return PartialView("Home/_Posts", post);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddPostReport(PostReportVM postReportVm)
    {
        int? userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        await _postService.ReportPostAsync(postReportVm.PostId, userId.Value);
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVm) 
    {
        await _postService.RemovePostCommentAsync(removeCommentVm.CommentId);

        var post= await _postService.GetPostByIdAsync(removeCommentVm.PostId);
        
        return PartialView("Home/_Posts", post);
    }

    [HttpPost]
    public async Task<IActionResult> PostDelete(PostRemoveVM postRemoveVm)
    {
        var removedPost = await _postService.RemovePostAsync(postRemoveVm.PostId);
        await _hashtagService.RemoveHashtags(removedPost.Content);
        
        return RedirectToAction("Index");
    }
}