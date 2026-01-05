using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Base;
using WebApplication.Data.Helpers.Constants;
using WebApplication.Data.Helpers.Enums;
using WebApplication.Data.Models;
using WebApplication.Data.Services;
using WebApplication.ViewModels.Settings;

namespace WebApplication.Controllers;
[Authorize]
public class SettingsController(IUserService userService, IFileService fileService, UserManager<User> userManager) : BaseController
{
    private readonly IUserService _userService = userService;
    private readonly IFileService _fileService = fileService;
    private readonly UserManager<User> _userManager = userManager;
    // GET
    public async Task<IActionResult> Index()
    {
        var loggedInUser = await _userManager.GetUserAsync(User);
        
        return View(loggedInUser);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfilePicture(ProfilePictureVM profilePictureVm)
    {
        int? loggedInUserId = GetUserId();
        if(loggedInUserId == null) return RedirectToLogin();
        var loggedInUser = await _userManager.GetUserAsync(User);
        var uploadedProfilePictureUrl =
            await _fileService.UploadImageAsync(profilePictureVm.ProfilePictureImage, ImageFileType.ProfileImage);
        await _userService.UpdateUserProfilePicture(loggedInUserId.Value, uploadedProfilePictureUrl);
       
        var userClaims = await _userManager.GetClaimsAsync(loggedInUser); 

        if(!userClaims.Any(c => c.Type == CustomClaim.ImageUrl))
            await _userManager.AddClaimAsync(loggedInUser, new Claim(CustomClaim.ImageUrl, loggedInUser.ProfilePictureUrl ?? Url.Content("~/images/avatar/user.png")));
        else await _userManager.ReplaceClaimAsync(loggedInUser, userClaims.FirstOrDefault(c => c.Type == CustomClaim.ImageUrl) , new Claim(CustomClaim.ImageUrl, loggedInUser.ProfilePictureUrl));
        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> UpdateCoverPicture(UpdateCoverAndPfpVm coverPictureVm)
    {
        int? loggedInUserId = GetUserId();
        if(loggedInUserId == null) return RedirectToLogin();
        var uploadedCoverPictureUrl =
            await _fileService.UploadImageAsync(coverPictureVm.CoverPictureImage, ImageFileType.CoverImage);
        await _userService.UpdateUserCoverPicture(loggedInUserId.Value, uploadedCoverPictureUrl);
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateImages(UpdateCoverAndPfpVm model)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToLogin();
        var user = await _userManager.GetUserAsync(User);
        var profilePicture = model.ProfilePictureImage;
        if (profilePicture != null)
        {
            var uploadedProfilePictureUrl =
                await _fileService.UploadImageAsync(profilePicture, ImageFileType.ProfileImage);
            await _userService.UpdateUserProfilePicture(userId.Value, uploadedProfilePictureUrl);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var pfpClaim = userClaims.FirstOrDefault(c => c.Type == CustomClaim.ImageUrl);
            if (pfpClaim == null)
            {
                await _userManager.AddClaimAsync(user, new Claim(CustomClaim.ImageUrl, user.ProfilePictureUrl ?? Url.Content("~/images/avatar/user.png")));
            }
            else await _userManager.ReplaceClaimAsync(user, pfpClaim , new Claim(CustomClaim.ImageUrl, user.ProfilePictureUrl?? Url.Content("~/images/avatar/user.png")));
        }
        
            
        var coverPicture = model.CoverPictureImage;
        if (coverPicture != null)
        {
            var uploadedCoverPictureUrl = await _fileService.UploadImageAsync(coverPicture, ImageFileType.CoverImage);
            await _userService.UpdateUserCoverPicture(userId.Value, uploadedCoverPictureUrl);
        }

        return Redirect($"/User/Details?userId={userId}");
        
    }
}

