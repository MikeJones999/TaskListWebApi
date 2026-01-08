using Microsoft.AspNetCore.Http;

namespace TaskList.Api.Application.Interfaces.Services
{
    public interface IBlobServices
    {
        Task UploadAsync(IFormFile file, string blobName);
        Task DeleteFileAsync(string blobName);
    }
}
