using AElf;
using AElf.Client.Dto;
using AElf.Cryptography.ECDSA;
using AElf.Types;
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

        public async Task<int> GetChainIdAsync()
        {
            return await _aelfClientFactory.CreateClient().GetChainIdAsync();
        }

        public async Task<MerklePathDto> GetMerklePathByTransactionIdAsync(string transactionId)
        {
            return await _aelfClientFactory.CreateClient().GetMerklePathByTransactionIdAsync(transactionId);
        }

        public async Task<string> SendTransactionAsync(ECKeyPair keyPair, string contract, string method, string @params = null)
        {
            var fromAddress = Address.FromPublicKey(keyPair.PublicKey);

            var contractAddress = await GetContractAddressAsync(contract);
            var rawTransaction = await GenerateRawTransactionAsync(fromAddress.ToBase58(), contractAddress, method, FormatParams(@params));
            var signature = GetSignature(keyPair, rawTransaction);

            var rawTransactionResult = await _aelfClientFactory.CreateClient().SendRawTransactionAsync(new SendRawTransactionInput()
            {
                Transaction = rawTransaction,
                Signature = signature,
            });

            return rawTransactionResult.TransactionId;
        }

        public async Task<string> SendTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params)
        {
            var fromAddress = Address.FromPublicKey(keyPair.PublicKey);

            var contractAddress = await GetContractAddressAsync(contract);
            var tx = await _aelfClientFactory.CreateClient().GenerateTransactionAsync(fromAddress.ToBase58(), contractAddress, method, @params);
            var txWithSign = GetTransactionWithSignature(keyPair, tx);

            var rawTransactionResult = await _aelfClientFactory.CreateClient().SendTransactionAsync(new SendTransactionInput()
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            return rawTransactionResult.TransactionId;
        }

        public async Task<string> CallTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params)
        {
            var fromAddress = Address.FromPublicKey(keyPair.PublicKey);

            var contractAddress = await GetContractAddressAsync(contract);
            var tx = await _aelfClientFactory.CreateClient().GenerateTransactionAsync(fromAddress.ToBase58(), contractAddress, method, @params);
            var txWithSign = GetTransactionWithSignature(keyPair, tx);

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

        private string GetSignature(ECKeyPair keyPair, string rawTransaction)
        {
            var transactionId = HashHelper.ComputeFrom(ByteArrayHelper.HexStringToByteArray(rawTransaction));
            var signature = _accountsService.Sign(keyPair, transactionId.ToByteArray());
            return ByteString.CopyFrom(signature).ToHex();
        }

        private Transaction GetTransactionWithSignature(ECKeyPair keyPair, Transaction transaction)
        {
            byte[] hash = transaction.GetHash().ToByteArray();
            byte[] bytes = _accountsService.Sign(keyPair, hash);
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
