using Microsoft.AspNetCore.Http;
using WebApplication.Data.Models;

namespace WebApplication.Data.Services;

public interface IStoryService
{
    Task<List<Story>> GetAllStoriesAsync();
    Task<Story> CreateStoryAsync(Story story);
}