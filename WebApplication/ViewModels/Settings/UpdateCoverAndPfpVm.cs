namespace WebApplication.ViewModels.Settings;

public class UpdateCoverAndPfpVm
{
    public IFormFile? CoverPictureImage { get; set; }
    public IFormFile? ProfilePictureImage { get; set; }
}