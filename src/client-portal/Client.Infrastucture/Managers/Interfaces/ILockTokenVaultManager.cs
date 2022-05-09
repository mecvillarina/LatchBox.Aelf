using AElf.Client.Dto;
using AElf.Client.LatchBox.LockTokenVault;
using Client.Infrastructure.Models;
using Client.Infrastructure.Models.Inputs;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface ILockTokenVaultManager : IManager
    {
        string ContactAddress { get; }
        Task<TransactionResultDto> InitializeAsync(WalletInformation wallet, string password);
        Task<TransactionResultDto> AddLockAsync(WalletInformation wallet, string password, AddLockInputModel model);
        Task<TransactionResultDto> ClaimLockAsync(WalletInformation wallet, string password, long lockId);
        Task<TransactionResultDto> RevokeLockAsync(WalletInformation wallet, string password, long lockId);
        Task<TransactionResultDto> ClaimRefundAsync(WalletInformation wallet, string password, string tokenSymbol);

        Task<Int64Value> GetLocksCountAsync(WalletInformation wallet, string password);
        Task<GetLockTransactionOutput> GetLockTransactionAsync(WalletInformation wallet, string password, long lockId);
        Task<GetLockListOutput> GetLocksAsync(WalletInformation wallet, string password);
        Task<GetLockListOutput> GetLocksByInitiatorAsync(WalletInformation wallet, string password, string initiator);
        Task<GetLockReceiverListOutput> GetLocksForReceiverAsync(WalletInformation wallet, string password, string receiver);
        Task<GetLockListOutput> GetLocksByAssetAsync(WalletInformation wallet, string password, string tokenSymbol);
        Task<GetRefundListOutput> GetRefundsAsync(WalletInformation wallet, string password);
        Task<GetLockAssetCounterListOutput> GetAssetsCounterAsync(WalletInformation wallet, string password);

    }
}