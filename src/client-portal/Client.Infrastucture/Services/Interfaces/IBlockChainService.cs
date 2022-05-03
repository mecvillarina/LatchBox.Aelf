using AElf.Client.Dto;
using System.Threading.Tasks;

namespace Client.Infrastructure.Services.Interfaces
{
    public interface IBlockChainService
    {
        Task<string> SendTransactionAsync(string account, string password, string contract, string method, string @params = null);
        Task<string> ExecuteTransactionAsync(string account, string password, string contract, string method, string @params = null);
        Task<TransactionResultDto> CheckTransactionResultAsync(string txId);

    }
}