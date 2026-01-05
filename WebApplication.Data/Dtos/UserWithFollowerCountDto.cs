using WebApplication.Data.Models;

namespace WebApplication.Data.Dtos;

public class UserWithFollowerCountDto
{
    public User User {get; set;}
    public int FollowerCount {get; set;}
}