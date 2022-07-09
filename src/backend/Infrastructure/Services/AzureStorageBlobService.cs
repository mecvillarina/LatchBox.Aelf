using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Application.Common.Interfaces;
using Application.Common.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Infrastructure.Services
{
    public class AzureStorageBlobService : IAzureStorageBlobService
    {
        private readonly IAzureStorageAccountService _azureStorageAccountService;
        private readonly IDateTime _dateTime;
        public AzureStorageBlobService(IDateTime dateTime, IAzureStorageAccountService azureStorageAccountService)
        {
            _dateTime = dateTime;
            _azureStorageAccountService = azureStorageAccountService;
        }

        private BlobContainerClient GetBlobContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = new(_azureStorageAccountService.StorageConnectionString, containerName);

            if (!blobContainerClient.Exists())
            {
                blobContainerClient.Create();
                blobContainerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            }

            return blobContainerClient;
        }

        private BlobClient GetBlobClient(string containerName, string blobName) => new(_azureStorageAccountService.StorageConnectionString, containerName, blobName);

        public string GetBlobContainerPath(string containerName)
        {
            var container = GetBlobContainer(containerName);
            return container.Uri.AbsoluteUri;
        }

        public Uri GetServiceSasUriForBlob(string blobContainerName, string blobName, double expiresAfterInMinutes = 30)
        {
            BlobClient blobClient = GetBlobClient(blobContainerName, blobName);

            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                sasBuilder.StartsOn = _dateTime.UtcNow;
                sasBuilder.ExpiresOn = _dateTime.UtcNow.AddMinutes(expiresAfterInMinutes);
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                return sasUri;
            }
            else
            {
                return null;
            }
        }

        public Uri GetServiceSasUriForContainer(string blobContainerName, double expiresAfterInMinutes = 30)
        {
            var containerClient = GetBlobContainer(blobContainerName);

            if (containerClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "c"
                };

                sasBuilder.StartsOn = _dateTime.UtcNow;
                sasBuilder.ExpiresOn = _dateTime.UtcNow.AddMinutes(expiresAfterInMinutes);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

                Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
                return sasUri;
            }
            else
            {
                return null;
            }
        }

        public async Task<IResult> UploadAsync(Stream stream, string containerName, string filename)
        {
            var container = GetBlobContainer(containerName);

            stream.Seek(0, SeekOrigin.Begin);
            var blockBlob = container.GetBlobClient(filename);

            string contentType = string.Empty;
            var blobHttpHeader = new BlobHttpHeaders();
            string extension = Path.GetExtension(blockBlob.Uri.AbsoluteUri);
            contentType = extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                ".json" => "application/json",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                _ => "application/octet-stream",
            };
            blobHttpHeader.ContentType = contentType;
            await blockBlob.UploadAsync(stream, blobHttpHeader);
            return await Result.SuccessAsync();
        }

        public async Task<Result<string>> CopyAsync(string sourceContainerName, string sourceBlobName, string destinationContainerName)
        {

            var sourceContainer = GetBlobContainer(sourceContainerName);
            BlobClient sourceBlob = sourceContainer.GetBlobClient(sourceBlobName);

            if (await sourceBlob.ExistsAsync())
            {
                BlobLeaseClient lease = sourceBlob.GetBlobLeaseClient();

                await lease.AcquireAsync(TimeSpan.FromSeconds(-1));

                BlobProperties sourceProperties = await sourceBlob.GetPropertiesAsync();
                Console.WriteLine($"Lease state: {sourceProperties.LeaseState}");

                var destinationContainer = GetBlobContainer(destinationContainerName);

                string destinationBlobName = Guid.NewGuid().ToString().Split('-').Last() + "-" + sourceBlob.Name;
                BlobClient destBlob = destinationContainer.GetBlobClient(destinationBlobName);

                await destBlob.StartCopyFromUriAsync(sourceBlob.Uri);

                sourceProperties = await sourceBlob.GetPropertiesAsync();

                if (sourceProperties.LeaseState == LeaseState.Leased)
                {
                    await lease.BreakAsync();
                }

                return await Result<string>.SuccessAsync(data: destinationBlobName);
            }

            return await Result<string>.FailAsync();
        }

        public async Task<IResult> DeleteAsync(string containerName, string filename)
        {
            var container = GetBlobContainer(containerName);
            var blockBlob = container.GetBlobClient(filename);
            await blockBlob.DeleteIfExistsAsync();
            return await Result.SuccessAsync();
        }
    }
}
