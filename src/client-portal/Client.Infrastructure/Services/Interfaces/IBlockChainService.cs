using AElf.Client.Dto;
using AElf.Cryptography.ECDSA;
using Google.Protobuf;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IBlockChainService
    {
        Task<int> GetMainChainIdAsync();
        Task<ChainStatusDto> GetMainChainStatusAsync();
        Task<int> GetSideChainIdAsync();
        Task<ChainStatusDto> GetSideChainStatusAsync();
        Task<MerklePathDto> GetMainChainMerklePathByTransactionIdAsync(string transactionId);
        Task<MerklePathDto> GetSideChainMerklePathByTransactionIdAsync(string transactionId);

        Task<(string, string)> SendMainChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params);
        Task<string> SendSideChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params);
        Task<string> CallMainChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params, ChainStatusDto chainStatus = null);
        Task<string> CallSideChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params, ChainStatusDto chainStatus = null);
        Task<TransactionResultDto> CheckMainChainTransactionResultAsync(string txId);
        Task<TransactionResultDto> CheckSideChainTransactionResultAsync(string txId);
        Task<string> GetMainChainContractAddressAsync(string contract);
        Task<string> GetSideChainContractAddressAsync(string contract);

    }
}