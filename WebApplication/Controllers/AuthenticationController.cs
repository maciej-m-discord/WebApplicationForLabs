using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Models;
using WebApplication.ViewModels.Authentication;
using WebApplication.ViewModels.Settings;

namespace WebApplication.Controllers;

public class AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManger, SignInManager<User> signInManager) : Controller
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    // GET
    public async Task<IActionResult> Login()
    {
        return View();
    }
    
    [HttpPost] 
    public async Task<IActionResult> Login(LoginVm loginVm)
    {
        if (!ModelState.IsValid)
            return View(loginVm);
        var loggedInUser = await _userManager.FindByEmailAsync(loginVm.Email);
        if (loggedInUser == null)
        {
            ModelState.AddModelError("", "No user found with the provided email and password.");
            return View(loginVm);
        }
        
        var userClaims = await _userManager.GetClaimsAsync(loggedInUser);
        var fullNameClaim = userClaims.FirstOrDefault(c => c.Type == CustomClaim.FullName);
        var imageUrlClaim = userClaims.FirstOrDefault(c => c.Type == CustomClaim.ImageUrl);
        if(fullNameClaim == null)
            await _userManager.AddClaimAsync(loggedInUser, new Claim(CustomClaim.FullName, loggedInUser.FullName));
        else await _userManager.ReplaceClaimAsync(loggedInUser, fullNameClaim , new Claim(CustomClaim.FullName, loggedInUser.FullName));
        if(imageUrlClaim == null)
            await _userManager.AddClaimAsync(loggedInUser, new Claim(CustomClaim.ImageUrl, loggedInUser.ProfilePictureUrl ?? Url.Content("~/images/avatar/user.png")));
        else await _userManager.ReplaceClaimAsync(loggedInUser, imageUrlClaim , new Claim(CustomClaim.ImageUrl, loggedInUser.ProfilePictureUrl ?? Url.Content("~/images/avatar/user.png")));
        
        var result = await _signInManager.PasswordSignInAsync(loggedInUser.UserName, loginVm.Password, false, false);
        
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        
        ModelState.AddModelError(string.Empty, "The email address and passwords do not match, try again.");
        return View(loginVm);
    }

    public async Task<IActionResult> Register()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm registerVm)
    {
        if (!ModelState.IsValid)
            return View(registerVm);
        
        var newUser= new User
        {
            FullName = registerVm.FirstName + " " + registerVm.LastName,
            Email = registerVm.Email,
            UserName = registerVm.Email
        };
        
        var existingUser = await _userManager.FindByEmailAsync(registerVm.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Email is already in use.");
            return View();
        }
        
        var result = await _userManager.CreateAsync(newUser, registerVm.Password);
        if (result.Succeeded)
        {  
            await _userManager.AddToRoleAsync(newUser, AppRoles.User);
            
            await _userManager.AddClaimAsync(newUser, new Claim(CustomClaim.FullName, newUser.FullName));
            await _userManager.AddClaimAsync(newUser, new Claim(CustomClaim.ImageUrl, newUser.ProfilePictureUrl?? Url.Content("/images/avatar/user.png")));
            
            await _signInManager.SignInAsync(newUser, isPersistent: false);
            
            return RedirectToAction("Index", "Home");
        }
        
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(registerVm);
    }
    [HttpPost]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordVM updatePasswordVm)
    {
        if (updatePasswordVm.NewPassword != updatePasswordVm.ConfirmPassword)
        {
            TempData["PasswordError"] = "Passwords do not match.";
            TempData["ActiveTab"] = "Password";
            return RedirectToAction("Index", "Settings");
        }
        
        var loggedInUser = await _userManager.GetUserAsync(User);
        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(loggedInUser, updatePasswordVm.CurrentPassword);
        if (!isCurrentPasswordValid)
        {
            TempData["PasswordError"] = "Current password is incorrect.";
            TempData["ActiveTab"] = "Password";
            return RedirectToAction("Index", "Settings");
        }

        var result = await _userManager.ChangePasswordAsync(loggedInUser, updatePasswordVm.CurrentPassword, updatePasswordVm.NewPassword);
        if (!result.Succeeded)
        {
            TempData["PasswordSuccess"] = "Password updated successfully.";
            TempData["ActiveTab"] = "Password";
            
            await _signInManager.RefreshSignInAsync(loggedInUser);
        }
        return RedirectToAction("Index", "Settings");
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UpdateProfileVM updateProfileVm)
    {
        var loggedInUser = await _userManager.GetUserAsync(User);
        if (loggedInUser == null)
            return RedirectToAction("Login");

        loggedInUser.FullName = updateProfileVm.FullName;
        loggedInUser.UserName = updateProfileVm.UserName;
        loggedInUser.Bio = updateProfileVm.Bio;

        var result  = await _userManager.UpdateAsync(loggedInUser);
        if (!result.Succeeded)
        {
            
            TempData["UserProfileError"] = "Failed to update profile.";
            TempData["ActiveTab"] = "Profile";
        }
        var userClaims = await _userManager.GetClaimsAsync(loggedInUser);
        var fullNameClaim = userClaims.FirstOrDefault(c => c.Type == CustomClaim.FullName);
        
        if(fullNameClaim==null)
            await _userManager.AddClaimAsync(loggedInUser, new Claim(CustomClaim.FullName, loggedInUser.FullName));
        else await _userManager.ReplaceClaimAsync(loggedInUser, fullNameClaim , new Claim(CustomClaim.FullName, loggedInUser.FullName));
        
        await _signInManager.RefreshSignInAsync(loggedInUser);
        return RedirectToAction("Index", "Settings");
    }

    public IActionResult ExternalLogin(string provider)
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Authentication");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }
    
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var info = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (info == null)
            return RedirectToAction("Login");
        
        var emailClaim = info.Principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(emailClaim); 
        
        if (user == null)
        {
            var newUser = new User()
            {
                Email = emailClaim,
                UserName = emailClaim,
                FullName = info.Principal.FindFirstValue(ClaimTypes.Name),
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(newUser);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.User);
                await _userManager.AddClaimAsync(newUser, new Claim(CustomClaim.FullName, newUser.FullName));
                await _userManager.AddClaimAsync(newUser, new Claim(CustomClaim.ImageUrl, newUser.ProfilePictureUrl?? Url.Content("/images/avatar/user.png")));
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                
                return RedirectToAction("Index", "Home");
            }
        }
        else
        {
            var userClaims =  await _userManager.GetClaimsAsync(user);
            var fullNameClaim = userClaims.FirstOrDefault(c => c.Type == CustomClaim.FullName);
            var imageUrlClaim = userClaims.FirstOrDefault(c => c.Type == CustomClaim.ImageUrl);
            if (fullNameClaim == null)
                await _userManager.AddClaimAsync(user, new Claim(CustomClaim.FullName, user.FullName));
            else await _userManager.ReplaceClaimAsync(user, fullNameClaim , new Claim(CustomClaim.FullName, user.FullName));
            if (imageUrlClaim == null) 
                await _userManager.AddClaimAsync(user, new Claim(CustomClaim.ImageUrl, user.ProfilePictureUrl ?? Url.Content("/images/avatar/user.png")));
            else await _userManager.ReplaceClaimAsync(user, imageUrlClaim , new Claim(CustomClaim.ImageUrl, user.ProfilePictureUrl ?? Url.Content("/images/avatar/user.png")));
        }
        
        
        await _signInManager.SignInAsync(user, isPersistent: false);
        
        return RedirectToAction("Index", "Home");
    }
    
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
    
}