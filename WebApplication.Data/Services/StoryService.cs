using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public class StoryService(AppDbContext context):IStoryService
{
    private readonly AppDbContext _context = context;
    public async Task<List<Story>> GetAllStoriesAsync()
    {
        var allStories = await _context.Stories
            .Where(s => s.CreatedAt >= DateTime.UtcNow.AddHours(-24))
            .Include(s => s.User)
            .ToListAsync();
        return allStories;
    }

    public async Task<Story> CreateStoryAsync(Story story)
    {
        
        await _context.Stories.AddAsync(story);
        await _context.SaveChangesAsync();
        return story;
    }
}