using Microsoft.AspNetCore.Http;

namespace TaskList.Api.Application.Interfaces.Services
{
    public interface IFileServices
    {
        Task UploadAsync(IFormFile file, string fileName);
        Task<byte[]?> DownloadAsync(string fileName);
        Task DeleteFileAsync(string storageName);
        Task<bool> FileExistsAsync(string storageName);
    }
}
