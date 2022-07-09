using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public interface IAzureStorageAccountService
    {
        string StorageConnectionString { get; }
    }
}
