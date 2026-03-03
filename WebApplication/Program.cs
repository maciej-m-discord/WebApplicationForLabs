using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Data.Helpers;
using WebApplication.Data.Models;
using WebApplication.Data.Services;
using WebApplication.Hubs;


var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

//Database Configuration
string dbConnectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(dbConnectionString));

var blobConnectionString = builder.Configuration["AzureStorageConnectionString"] ?? throw new InvalidOperationException("Connection string 'AzureBlobStorage' not found.");

//Services Configuration 
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IHashtagService, HashtagService>();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IFileService>(s => new FileService(blobConnectionString));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMutualService, MutualService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAdminService, AdminService>();

//Identity Configuration
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
    {
        //Password settings 
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 4;
        options.Password.RequiredUniqueChars = 1;
        
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authentication/Login";
    options.AccessDeniedPath = "/Authentication/AccessDenied";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
        options.CallbackPath = "/signin-google";
    }).AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"] ?? "";
        options.CallbackPath = "/signin-github";
    });
builder.Services.AddAuthorization();

builder.Services.AddSignalR();

var app = builder.Build();

//Seed the database with inital data.
 using (var scope = app.Services.CreateScope())
 {
     var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
     await dbContext.Database.MigrateAsync();
     await DbInitializer.SeedAsync(dbContext);
     
     var userManager =scope.ServiceProvider.GetRequiredService<UserManager<User>>();
     var roleManager =scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
     await DbInitializer.SeedUsersAndRolesAsync(userManager,roleManager);
 }


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();