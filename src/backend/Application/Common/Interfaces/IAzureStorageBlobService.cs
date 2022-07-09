using Application.Common.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IAzureStorageBlobService
    {
        string GetBlobContainerPath(string containerName);
        Task<IResult> UploadAsync(Stream stream, string containerName, string filename);
        Task<Result<string>> CopyAsync(string sourceContainerName, string sourceBlobName, string destinationContainerName);
        Uri GetServiceSasUriForBlob(string blobContainerName, string blobName, double expiresAfterInMinutes = 30);
        Uri GetServiceSasUriForContainer(string blobContainerName, double expiresAfterInMinutes = 30);
        Task<IResult> DeleteAsync(string containerName, string filename);
    }
}