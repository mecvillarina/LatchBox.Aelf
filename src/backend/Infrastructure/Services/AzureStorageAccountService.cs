using Application.Common.Constants;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class AzureStorageAccountService : IAzureStorageAccountService
    {
        public AzureStorageAccountService(IConfiguration configuration)
        {
            StorageConnectionString = configuration.GetValue<string>(EnvironmentVariableKeys.AZURESTORAGEACCOUNT);
        }

        public string StorageConnectionString { get; private set; }
    }
}
