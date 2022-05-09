using AElf;
using AElf.Client.Dto;
using AElf.Types;
using Client.Infrastructure.Models;
using Client.Infrastructure.Services.Interfaces;
using Google.Protobuf;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services
{
    public class BlockChainService : IBlockChainService
    {
        private readonly IAccountsService _accountsService;
        private readonly AElfClientFactory _aelfClientFactory;

        public BlockChainService(IAccountsService accountsService, AElfClientFactory aelfClientFactory)
        {
            _accountsService = accountsService;
            _aelfClientFactory = aelfClientFactory;
        }

        public async Task<string> SendTransactionAsync(WalletInformation wallet, string password, string contract, string method, string @params = null)
        {
            var contractAddress = await GetContractAddressAsync(contract);
            var rawTransaction = await GenerateRawTransactionAsync(wallet.Address, contractAddress, method, FormatParams(@params));
            var signature = await GetSignatureAsync(wallet.Filename, password, rawTransaction);

            var rawTransactionResult = await _aelfClientFactory.CreateClient().SendRawTransactionAsync(new SendRawTransactionInput()
            {
                Transaction = rawTransaction,
                Signature = signature,
            });

            return rawTransactionResult.TransactionId;
        }

        public async Task<string> SendTransactionAsync(WalletInformation wallet, string password, string contract, string method, IMessage @params)
        {
            var contractAddress = await GetContractAddressAsync(contract);
            var tx = await _aelfClientFactory.CreateClient().GenerateTransactionAsync(wallet.Address, contractAddress, method, @params);
            var txWithSign = await GetTransactionWithSignatureAsync(wallet.Filename, password, tx);

            var rawTransactionResult = await _aelfClientFactory.CreateClient().SendTransactionAsync(new SendTransactionInput()
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            return rawTransactionResult.TransactionId;
        }

        public async Task<string> CallTransactionAsync(WalletInformation wallet, string password, string contract, string method, IMessage @params)
        {
            var contractAddress = await GetContractAddressAsync(contract);
            var tx = await _aelfClientFactory.CreateClient().GenerateTransactionAsync(wallet.Address, contractAddress, method, @params);
            var txWithSign = await GetTransactionWithSignatureAsync(wallet.Filename, password, tx);

            var rawTransactionResult = await _aelfClientFactory.CreateClient().ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            return rawTransactionResult;
        }

        public async Task<TransactionResultDto> CheckTransactionResultAsync(string txId)
        {
            var client = _aelfClientFactory.CreateClient();

            var result = await client.GetTransactionResultAsync(txId);
            var i = 0;
            while (i < 10)
            {
                if (result.Status == TransactionResultStatus.Mined.ToString().ToUpper())
                {
                    break;
                }

                if (result.Status == TransactionResultStatus.Failed.ToString().ToUpper() || result.Status == TransactionResultStatus.NodeValidationFailed.ToString().ToUpper())
                {
                    break;
                }

                await Task.Delay(1000);
                result = await client.GetTransactionResultAsync(txId);
                i++;
            }

            return result;
        }

        private async Task<string> GetSignatureAsync(string keyStoreFile, string password, string rawTransaction)
        {
            var transactionId = HashHelper.ComputeFrom(ByteArrayHelper.HexStringToByteArray(rawTransaction));
            var signature = await _accountsService.SignAsync(keyStoreFile, password,
                transactionId.ToByteArray());
            return ByteString.CopyFrom(signature).ToHex();
        }

        private async Task<Transaction> GetTransactionWithSignatureAsync(string keyStoreFile, string password, Transaction transaction)
        {
            byte[] hash = transaction.GetHash().ToByteArray();
            byte[] bytes = await _accountsService.SignAsync(keyStoreFile, password, hash);
            transaction.Signature = ByteString.CopyFrom(bytes);
            return transaction;
        }

        private async Task<string> GetContractAddressAsync(string contract)
        {
            var contractAddress = contract;
            if (contract.StartsWith("AElf.ContractNames."))
            {
                contractAddress = (await _aelfClientFactory.CreateClient().GetContractAddressByNameAsync(HashHelper.ComputeFrom(contract))).ToBase58();
            }

            return contractAddress;
        }

        private async Task<string> GenerateRawTransactionAsync(string from, string to, string method, string @params)
        {
            var client = _aelfClientFactory.CreateClient();
            var status = await client.GetChainStatusAsync();
            var height = status.BestChainHeight;
            var blockHash = status.BestChainHash;

            var rawTransaction = await client.CreateRawTransactionAsync(new CreateRawTransactionInput
            {
                From = from,
                To = to,
                MethodName = method,
                Params = @params,
                RefBlockNumber = height,
                RefBlockHash = blockHash
            });

            return rawTransaction.RawTransaction;
        }

        private string FormatParams(string @params)
        {
            if (string.IsNullOrWhiteSpace(@params))
            {
                @params = "{}";
            }

            var json = JsonConvert.DeserializeObject(@params);
            return JsonConvert.SerializeObject(json);
        }
    }
}
