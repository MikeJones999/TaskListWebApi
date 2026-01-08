using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskList.Api.Application.Interfaces.Services;

namespace TaskList.Api.Infrastructure.Services.FileStorage
{
    public class FileService : IFileServices
    {
        private readonly ILogger<FileService> _logger;
        private readonly string _storagePath;

        public FileService(ILogger<FileService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _storagePath = configuration["FileStorage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(_storagePath))
            {
                _logger.LogInformation("Creating local file storage directory at {StoragePath}", _storagePath);
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task UploadAsync(IFormFile file, string fileName)
        {
            _logger.LogInformation("Uploading file {StorageName} to local storage", fileName);

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Attempted to upload null or empty file with storage name {StorageName}", fileName);
                throw new ArgumentException("File is null or empty", nameof(file));
            }

            try
            {
                string filePath = Path.Combine(_storagePath, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("Successfully uploaded file {StorageName} ({Size} bytes) to {FilePath}", fileName, file.Length, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {StorageName} to local storage", fileName);
                throw;
            }
        }

        public async Task<byte[]?> DownloadAsync(string fileName)
        {
            _logger.LogInformation("Downloading file {StorageName} from local storage", fileName);

            try
            {
                string filePath = Path.Combine(_storagePath, fileName);

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File {StorageName} not found at {FilePath}", fileName, filePath);
                    return null;
                }

                byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                _logger.LogInformation("Successfully downloaded file {StorageName} ({Size} bytes)", fileName, fileBytes.Length);
                return fileBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file {StorageName} from local storage", fileName);
                throw;
            }
        }

        public Task DeleteFileAsync(string storageName)
        {
            _logger.LogInformation("Deleting file {StorageName} from local storage", storageName);

            try
            {
                string filePath = Path.Combine(_storagePath, storageName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Successfully deleted file {StorageName} from {FilePath}", storageName, filePath);
                }
                else
                {
                    _logger.LogWarning("File {StorageName} not found for deletion at {FilePath}", storageName, filePath);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {StorageName} from local storage", storageName);
                throw;
            }
        }

        public Task<bool> FileExistsAsync(string storageName)
        {
            try
            {
                string filePath = Path.Combine(_storagePath, storageName);
                bool exists = File.Exists(filePath);
                _logger.LogDebug("File {StorageName} exists: {Exists}", storageName, exists);
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if file {StorageName} exists", storageName);
                throw;
            }
        }
    }
}
