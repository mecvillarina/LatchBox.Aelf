using AElf.Client.Dto;
using AElf.Cryptography.ECDSA;
using Google.Protobuf;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IBlockChainService
    {
        Task<int> GetMainChainIdAsync();
        Task<int> GetSideChainIdAsync();
        Task<MerklePathDto> GetMainChainMerklePathByTransactionIdAsync(string transactionId);
        Task<MerklePathDto> GetSideChainMerklePathByTransactionIdAsync(string transactionId);

        Task<string> SendMainChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params);
        Task<string> SendSideChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params);
        Task<string> CallMainChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params);
        Task<string> CallSideChainTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params);
        Task<TransactionResultDto> CheckMainChainTransactionResultAsync(string txId);
        Task<TransactionResultDto> CheckSideChainTransactionResultAsync(string txId);
    }
}