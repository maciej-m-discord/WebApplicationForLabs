namespace WebApplication.Data.Services;

public interface IHashtagService
{
    Task ProcessHashtags(string content);
    Task RemoveHashtags(string content);
}