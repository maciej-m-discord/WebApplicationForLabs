using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Data.Helpers.Enums;

namespace WebApplication.Data.Services;

public class FileService(string connectionString) : IFileService
{
    private readonly BlobServiceClient _blobServiceClient = new(connectionString);

    public async Task<string> UploadImageAsync(IFormFile file, ImageFileType imageFileType)
    {
        string containerName = imageFileType switch
        {
            ImageFileType.PostImage => "posts",
            ImageFileType.StoryImage => "stories",
            ImageFileType.ProfileImage => "profiles",
            ImageFileType.CoverImage => "covers",
            _ => throw new ArgumentException("Invalid image file type", nameof(imageFileType))
        };
        
        if(file== null || file.Length == 0)
            return "";
        
        //ensure the container exists

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        
        //generate a unique name for the file
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var blobClient = containerClient.GetBlobClient(fileName);
        
        //upload the file
        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            });
        }
        
        return blobClient.Uri.ToString();
    }
}