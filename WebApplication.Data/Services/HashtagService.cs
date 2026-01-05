using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Helpers;
using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public class HashtagService(AppDbContext context) :IHashtagService
{
    private readonly AppDbContext _context = context;
    public async Task ProcessHashtags(string content)
    {
        var postHashtags = HashtagHelper.GetHashtags(content);
        foreach (var hashtag in postHashtags)
        {
            var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);
            if (hashtagDb != null)
            {
                hashtagDb.Count++;
                hashtagDb.DateUpdated = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
            }
            else
            {
                var newHashtag = new Hashtag()
                {
                    Name = hashtag,
                    Count = 1,
                    CreatedAt = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };
                await _context.Hashtags.AddAsync(newHashtag);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task RemoveHashtags(string content)
    { 
        var postHashtags = HashtagHelper.GetHashtags(content);
        foreach (var hashtag in postHashtags)
        {   
            var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);
            if (hashtagDb != null)
            {
                hashtagDb.Count--;
                hashtagDb.DateUpdated = DateTime.UtcNow;
        
                _context.Hashtags.Update(hashtagDb);
                await _context.SaveChangesAsync();
            }
        }
    }
}