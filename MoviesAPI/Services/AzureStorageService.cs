//using Microsoft.Azure.Storage.Blob;
//using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MoviesAPI.Services
{
    public class AzureStorageService : IFileStorageService
    {
        private readonly string connectionString;

        public AzureStorageService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorageConnection");
        }

        public async Task DeleteFile(string fileRoute, string containerName)
        {
            if (fileRoute != null)
            {
                BlobServiceClient blobServiceClient = new(connectionString);
                var container = blobServiceClient.GetBlobContainerClient(containerName);

                var blobName = Path.GetFileName(fileRoute);
                var blob = container.GetBlobClient(blobName);
                await blob.DeleteIfExistsAsync();
            }
        }

        public async Task<string> EditFile(byte[] content, string extension, string containerName, string fileRoute, string contentType)
        {
            await DeleteFile(fileRoute, containerName);
            return await SaveFile(content, extension, containerName, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName, string contentType)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            var container = blobServiceClient.GetBlobContainerClient(containerName);

            await container.CreateIfNotExistsAsync();
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);
            string fileName = $"{Guid.NewGuid()}{extension}";

            var blob = container.GetBlobClient(fileName);

            await blob.UploadAsync(new MemoryStream(content),
                 new BlobHttpHeaders()
                 {
                     ContentType = contentType,
                 });
            return blob.Uri.ToString();
        }
    }
}
