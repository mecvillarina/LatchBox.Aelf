using AElf.Client.Dto;
using AElf.Cryptography.ECDSA;
using Google.Protobuf;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IBlockChainService
    {
        Task<int> GetChainIdAsync();
        Task<MerklePathDto> GetMerklePathByTransactionIdAsync(string transactionId);

        Task<string> SendTransactionAsync(ECKeyPair keyPair, string contract, string method, string @params = null);
        Task<string> SendTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params);
        Task<string> CallTransactionAsync(ECKeyPair keyPair, string contract, string method, IMessage @params);
        Task<TransactionResultDto> CheckTransactionResultAsync(string txId);
    }
}