using Microsoft.AspNetCore.Http;
using WebApplication.Data.Helpers.Enums;

namespace WebApplication.Data.Services;

public interface IFileService
{
    Task<string> UploadImageAsync(IFormFile file, ImageFileType fileType);
}