using AElf;
using AElf.Client.Dto;
using AElf.Cryptography.ECDSA;
using AElf.Types;
using Client.Infrastructure.Services.Interfaces;
using Google.Protobuf;
using System;
using System.Diagnostics;
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

        public async Task<int> GetMainChainIdAsync()
        {
            return await _aelfClientFactory.CreateMainChainNodeClient().GetChainIdAsync();
        }

        public async Task<ChainStatusDto> GetMainChainStatusAsync()
        {
            return await _aelfClientFactory.CreateMainChainNodeClient().GetChainStatusAsync();
        }

        public async Task<int> GetSideChainIdAsync()
        {
            return await _aelfClientFactory.CreateSideChainNodeClient().GetChainIdAsync();
        }

        public async Task<ChainStatusDto> GetSideChainStatusAsync()
        {
            return await _aelfClientFactory.CreateSideChainNodeClient().GetChainStatusAsync();
        }

        public async Task<MerklePathDto> GetMainChainMerklePathByTransactionIdAsync(string transactionId)
        {
            return await _aelfClientFactory.CreateMainChainNodeClient().GetMerklePathByTransactionIdAsync(transactionId);
        }

        public async Task<MerklePathDto> GetSideChainMerklePathByTransactionIdAsync(string transactionId)
        {
            return await _aelfClientFactory.CreateSideChainNodeClient().GetMerklePathByTransactionIdAsync(transactionId);
        }

        public async Task<(string, string)> SendMainChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params)
        {
            var fromAddress = Address.FromPublicKey(keyPair.PublicKey);

            var contractAddress = await GetMainChainContractAddressAsync(contract);
            Debug.WriteLine(contractAddress);

            var tx = await _aelfClientFactory.CreateMainChainNodeClient().GenerateTransactionAsync(fromAddress.ToBase58(), contractAddress, method, @params);
            var txWithSign = GetTransactionWithSignature(keyPair, tx);
            var rawTransaction = txWithSign.ToByteArray().ToHex();
            var rawTransactionResult = await _aelfClientFactory.CreateMainChainNodeClient().SendTransactionAsync(new SendTransactionInput()
            {
                RawTransaction = rawTransaction
            });

            return (rawTransactionResult.TransactionId, rawTransaction);
        }

        public async Task<string> SendSideChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params)
        {
            var fromAddress = Address.FromPublicKey(keyPair.PublicKey);

            var contractAddress = await GetSideChainContractAddressAsync(contract);
            Debug.WriteLine(contractAddress);

            var tx = await _aelfClientFactory.CreateSideChainNodeClient().GenerateTransactionAsync(fromAddress.ToBase58(), contractAddress, method, @params);
            var txWithSign = GetTransactionWithSignature(keyPair, tx);

            var rawTransactionResult = await _aelfClientFactory.CreateSideChainNodeClient().SendTransactionAsync(new SendTransactionInput()
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            return rawTransactionResult.TransactionId;
        }

        public async Task<string> CallMainChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params, ChainStatusDto chainStatus = null)
        {
            var fromAddress = Address.FromPublicKey(keyPair.PublicKey);

            var contractAddress = await GetMainChainContractAddressAsync(contract);
            Debug.WriteLine(contractAddress);
            var tx = await _aelfClientFactory.CreateMainChainNodeClient().GenerateTransactionAsync(fromAddress.ToBase58(), contractAddress, method, @params, chainStatus);
            var txWithSign = GetTransactionWithSignature(keyPair, tx);

            var rawTransactionResult = await _aelfClientFactory.CreateMainChainNodeClient().ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            return rawTransactionResult;
        }

        public async Task<string> CallSideChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params, ChainStatusDto chainStatus = null)
        {
            var fromAddress = Address.FromPublicKey(keyPair.PublicKey);

            var contractAddress = await GetSideChainContractAddressAsync(contract);
            Debug.WriteLine(contractAddress);

            var tx = await _aelfClientFactory.CreateSideChainNodeClient().GenerateTransactionAsync(fromAddress.ToBase58(), contractAddress, method, @params, chainStatus);
            var txWithSign = GetTransactionWithSignature(keyPair, tx);

            var rawTransactionResult = await _aelfClientFactory.CreateSideChainNodeClient().ExecuteTransactionAsync(new ExecuteTransactionDto
            {
                RawTransaction = txWithSign.ToByteArray().ToHex()
            });

            return rawTransactionResult;
        }

        public async Task<TransactionResultDto> CheckMainChainTransactionResultAsync(string txId)
        {
            var client = _aelfClientFactory.CreateMainChainNodeClient();

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

        public async Task<TransactionResultDto> CheckSideChainTransactionResultAsync(string txId)
        {
            var client = _aelfClientFactory.CreateSideChainNodeClient();

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

        private Transaction GetTransactionWithSignature(ECKeyPair keyPair, Transaction transaction)
        {
            byte[] hash = transaction.GetHash().ToByteArray();
            byte[] bytes = _accountsService.Sign(keyPair, hash);
            transaction.Signature = ByteString.CopyFrom(bytes);
            return transaction;
        }

        public async Task<string> GetSideChainContractAddressAsync(string contract)
        {
            var contractAddress = contract;
            if (contract.StartsWith("AElf.ContractNames."))
            {
                contractAddress = (await _aelfClientFactory.CreateSideChainNodeClient().GetContractAddressByNameAsync(HashHelper.ComputeFrom(contract))).ToBase58();
            }

            return contractAddress;
        }

        public async Task<string> GetMainChainContractAddressAsync(string contract)
        {
            var contractAddress = contract;
            if (contract.StartsWith("AElf.ContractNames."))
            {
                contractAddress = (await _aelfClientFactory.CreateMainChainNodeClient().GetContractAddressByNameAsync(HashHelper.ComputeFrom(contract))).ToBase58();
            }

            return contractAddress;
        }
    }
}
