using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Models;
namespace WebApplication.Data.Helpers;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext appDbContext, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        if (!appDbContext.Users.Any() && !appDbContext.Posts.Any() && !roleManager.Roles.Any())
        {
            foreach (var role in AppRoles.AllRoles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
            
            User newUser = new User();
            IdentityResult result;
            
            var userPassword = "1234";
            newUser = new User()
            {
                UserName = "john.smith",
                Email = "john.smith@gmail.com",
                FullName = "John Smith",
                ProfilePictureUrl =
                    "https://images.unsplash.com/photo-1771749141777-85a499d85dc3?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                EmailConfirmed = true
            };
            
            result = await userManager.CreateAsync(newUser, userPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(newUser, AppRoles.User);
            result = IdentityResult.Failed();
            
            userPassword="Smoki1421.!";
            newUser = new User()
            {
                UserName = "site.admin",
                Email = "pieczysty1421@gmail.com",
                FullName = "Maciej M",
                ProfilePictureUrl =
                    "https://images.unsplash.com/photo-1771030669953-f9eb204d8aab?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                EmailConfirmed = true
            };
            
            result = await userManager.CreateAsync(newUser, userPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(newUser, AppRoles.Admin);  
            result = IdentityResult.Failed();
            
            newUser = new User()
            {
                FullName = "Joe Doe",
                Email = "1234@gmail.com",
                ProfilePictureUrl =
                    "https://images.unsplash.com/photo-1772415912163-bd5fe16b8ff0?q=80&w=1074&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                UserName = "joe.doe",
                EmailConfirmed = true
            };
            result = await userManager.CreateAsync(newUser, userPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(newUser, AppRoles.User); 
            
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
                    "https://images.unsplash.com/photo-1772211506168-1cbfcb361be8?q=80&w=1287&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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