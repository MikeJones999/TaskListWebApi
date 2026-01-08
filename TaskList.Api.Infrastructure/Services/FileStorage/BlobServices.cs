using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskList.Api.Application.Interfaces.Services;

namespace TaskList.Api.Infrastructure.Services.FileStorage
{
    public class BlobServices : IBlobServices
    {
        private BlobContainerClient _client;
        private readonly ILogger<BlobServices> _logger;
        private readonly IWebHostEnvironment _env;

        public BlobServices(ILogger<BlobServices> logger, IWebHostEnvironment env, IConfiguration configuration)
        {
            _logger = logger;
            _env = env;
            _client = new BlobContainerClient(
            new Uri(configuration.GetSection("BlobStorage:UriString").Value!),
            new StorageSharedKeyCredential(configuration.GetSection("BlobStorage:AccountName").Value!,
                 configuration.GetSection("BlobStorage:AccountKey").Value!)
             );
        }

        public async Task UploadAsync(IFormFile file, string blobName)
        {
            BlobClient blobClient = _client.GetBlobClient(blobName);
            if (file.Length > 0)
            {
                _client.CreateIfNotExists();
                _client.SetAccessPolicy(PublicAccessType.BlobContainer);
                using Stream stream = file.OpenReadStream();
                try
                {
                    var content = await blobClient.UploadAsync(stream);
                    string fileUrl = blobClient.Uri.AbsoluteUri;
                }
                catch (RequestFailedException ex)
                {
                    _logger.LogWarning("WHF - Failed to upload stream file to blob storage. Error: {ErrorMessage}, Request {Method}", ex.Message, nameof(this.UploadAsync));
                    throw;
                }
                finally
                {
                    stream.Close();
                }
            }
            else
            {
                _logger.LogWarning("WHF - Failed to upload stream file to blob storage due to file being empty. Request {Method}", nameof(this.UploadAsync));
                throw new Exception("File is empty - Failed to upload file");
            }
        }

        public async Task DeleteFileAsync(string blobName)
        {
            BlobClient blobClient = _client.GetBlobClient(blobName);
            if (!string.IsNullOrWhiteSpace(blobName) && _client.Exists())
            {
                try
                {
                    var response = await blobClient.DeleteAsync();
                    if (response.IsError)
                    {
                        throw new RequestFailedException($"Failed to delete blob file from blob storage - {response.ReasonPhrase}");
                    }
                }
                catch (RequestFailedException ex)
                {
                    _logger.LogWarning("WHF - Failed to delete file from blob storage. Request {Method}. Error Message: {Error}", nameof(this.DeleteFileAsync), ex.Message);
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("WHF - Failed to delete file from blob storage due to blob name being empty. Request {Method}", nameof(this.DeleteFileAsync));
                throw new Exception("Failed to delete blob file from blob storage - blob name missing");
            }
        }

    }
}
