using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.ViewComponents;

public class HashtagsViewComponent(AppDbContext context) : ViewComponent
{
    private readonly AppDbContext _context = context;
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var oneWeekAgoNow = DateTime.UtcNow.AddDays(-7);
        var topThreeHashtags = await _context.Hashtags
            .Where(h=>h.CreatedAt >= oneWeekAgoNow)
            .OrderByDescending(h => h.Count)
            .Take(3)
            .ToListAsync();
        return View(topThreeHashtags);
    }
}