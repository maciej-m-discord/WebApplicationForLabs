using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Base;
using WebApplication.Data.Helpers.Enums;
using WebApplication.Data.Models;
using WebApplication.Data.Services;
using WebApplication.ViewModels.Stories;

namespace WebApplication.Controllers;
[Authorize]
public class StoriesController(IStoryService storyService, IFileService fileService) : BaseController
{
    private readonly IStoryService _storyService = storyService;
    private readonly IFileService _fileService = fileService;
    

    [HttpPost]
    public async Task<IActionResult> CreateStory(StoryVM storyVm)
    {
        // if (!ModelState.IsValid)
        // {
        //     TempData["ErrorMessage"] = "Invalid story data. Please try again.";
        //     return RedirectToAction("Index", "Home");
        // }
        int? loggedInUserId = GetUserId();
        if(loggedInUserId == null) return RedirectToLogin();
        
        var imageUploadPath = await _fileService.UploadImageAsync(storyVm.Image, ImageFileType.StoryImage);
        var newStory = new Story
        {
            CreatedAt = DateTime.UtcNow,
            UserId = loggedInUserId.Value,
            ImageUrl = imageUploadPath
        };
        await _storyService.CreateStoryAsync(newStory);
        return RedirectToAction("Index", "Home");
    }
}