using Application.Common.Interfaces;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace Infrastructure.Services
{
    public class AzureStorageQueueService : IAzureStorageQueueService
    {
        private readonly IAzureStorageAccountService _azureStorageAccountService;
        public AzureStorageQueueService(IAzureStorageAccountService azureStorageAccountService)
        {
            _azureStorageAccountService = azureStorageAccountService;
        }

        private QueueClient GetQueueClient(string queueName)
        {
            QueueClient queueClient = new QueueClient(_azureStorageAccountService.StorageConnectionString, queueName);
            queueClient.CreateIfNotExists();
            return queueClient;
        }

        public void InsertMessage(string queueName, string message)
        {
            var queueClient = GetQueueClient(queueName);
            var b = System.Text.Encoding.UTF8.GetBytes(message);
            var message64Base = System.Convert.ToBase64String(b);
            queueClient.SendMessage(message64Base);
        }

        public string GetMessage(string queueName)
        {
            var queueClient = GetQueueClient(queueName);
            if (queueClient.Exists())
            {
                QueueProperties properties = queueClient.GetProperties();

                if (properties.ApproximateMessagesCount > 0)
                {
                    var retrievedMessage = queueClient.ReceiveMessage();
                    string message = retrievedMessage.Value.Body.ToString();
                    queueClient.DeleteMessage(retrievedMessage.Value.MessageId, retrievedMessage.Value.PopReceipt);
                    return message;
                }

                return null;
            }

            return null;
        }
    }
}
