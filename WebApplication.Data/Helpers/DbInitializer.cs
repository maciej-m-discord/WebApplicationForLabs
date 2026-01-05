using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Models;
namespace WebApplication.Data.Helpers;

public static class DbInitializer
{
    public static async Task SeedUsersAndRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        if (!roleManager.Roles.Any())
        {
            foreach (var role in AppRoles.AllRoles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
            
            //Users with roles
            if (!userManager.Users.Any(n => !string.IsNullOrEmpty(n.Email)))
            {
                var userPassword = "P@ssword1";
                var newUser = new User()
                {
                    UserName = "maciej.mniszak",
                    Email = "maciej.m1421@gmail.com",
                    FullName = "Maciej Mniszak",
                    ProfilePictureUrl =
                        "https://cdn.bsky.app/img/avatar/plain/did:plc:qo4ymftjk57mgr6rjsfgydiw/bafkreiab3obpnwmezkzw6xjkctwomb7ipdjnvy7o274qq6suf6vsxbfteq@jpeg",
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(newUser, userPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newUser, AppRoles.User);  
                
                var newAdmin = new User()
                {
                    UserName = "klaus.colossus",
                    Email = "pieczysty1421@gmail.com",
                    FullName = "Klaus Colossus",
                    ProfilePictureUrl =
                        "https://cdn.bsky.app/img/avatar/plain/did:plc:qo4ymftjk57mgr6rjsfgydiw/bafkreiab3obpnwmezkzw6xjkctwomb7ipdjnvy7o274qq6suf6vsxbfteq@jpeg",
                    EmailConfirmed = true
                };
                
                var resultNewAdmin = await userManager.CreateAsync(newAdmin, userPassword);
                if (resultNewAdmin.Succeeded)
                    await userManager.AddToRoleAsync(newAdmin, AppRoles.Admin);  
            }
        }
    }
    public static async Task SeedAsync(AppDbContext appDbContext)
    {
        if (!appDbContext.Users.Any() && !appDbContext.Posts.Any())
        {
            var newUser = new User()
            {
                FullName = "Discord Colossus",
                ProfilePictureUrl =
                    "https://cdn.bsky.app/img/avatar/plain/did:plc:qo4ymftjk57mgr6rjsfgydiw/bafkreiab3obpnwmezkzw6xjkctwomb7ipdjnvy7o274qq6suf6vsxbfteq@jpeg"
            };
            await appDbContext.Users.AddAsync(newUser);
            await appDbContext.SaveChangesAsync();

            var newPostWithoutImage = new Post()
            {
                Content = "This is going to be our first post which is being loaded from the Azure database and it has been created using our test user.",
                ImageUrl = "",
                NrOfReports = 0,
                CreatedAt = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,

                UserId = newUser.Id
            };
            var newPostWithImage = new Post()
            {
                Content =
                    "This is going to be our first post which is being loaded from the Azure database and it has been created using our test user. This post has an image.",
                ImageUrl =
                    "https://image.civitai.com/xG1nkqKTMzGDvpLrqFT7WA/b603a563-3235-45b4-8b64-e6d2f6ddb27f/original=true,quality=90/114467364.jpeg",
                NrOfReports = 0,
                CreatedAt = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,

                UserId = newUser.Id
            };
            await appDbContext.Posts.AddRangeAsync(newPostWithoutImage, newPostWithImage);
            await appDbContext.SaveChangesAsync();
        }
    }
}