using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Data.Services;

namespace WebApplication.ViewComponents;

public class StoriesViewComponent(IStoryService storyService) :ViewComponent 
{
    private readonly IStoryService _storyService = storyService;
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var allStories = await _storyService.GetAllStoriesAsync();
        return View(allStories);
    }
}