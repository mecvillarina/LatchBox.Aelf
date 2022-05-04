using AElf.Client.Dto;
using Client.Infrastructure.Models;
using Google.Protobuf;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IBlockChainService
    {
        Task<string> SendTransactionAsync(WalletInformation wallet, string password, string contract, string method, string @params = null);
        Task<string> SendTransactionAsync(WalletInformation wallet, string password, string contract, string method, IMessage @params = null);
        Task<string> ExecuteTransactionAsync(WalletInformation wallet, string password, string contract, string method, string @params = null);
        Task<string> ExecuteTransactionAsync(WalletInformation wallet, string password, string contract, string method, IMessage @params = null);
        Task<TransactionResultDto> CheckTransactionResultAsync(string txId);

    }
}