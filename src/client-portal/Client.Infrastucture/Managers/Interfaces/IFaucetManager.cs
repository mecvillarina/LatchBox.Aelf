using AElf.Client.Dto;
using Client.Infrastructure.Models;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IFaucetManager : IManager
    {
        Task<TransactionResultDto> TakeAsync(WalletInformation wallet, string password, string symbol, long amount);
    }
}