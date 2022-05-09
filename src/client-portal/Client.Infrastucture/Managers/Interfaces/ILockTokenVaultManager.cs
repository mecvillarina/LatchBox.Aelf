using AElf.Client.Dto;
using AElf.Client.LatchBox.LockTokenVault;
using Client.Infrastructure.Models.Inputs;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface ILockTokenVaultManager : IManager
    {
        string ContactAddress { get; }
        Task<TransactionResultDto> InitializeAsync();
        Task<TransactionResultDto> AddLockAsync(AddLockInputModel model);
        Task<TransactionResultDto> ClaimLockAsync(long lockId);
        Task<TransactionResultDto> RevokeLockAsync(long lockId);
        Task<TransactionResultDto> ClaimRefundAsync(string tokenSymbol);

        Task<Int64Value> GetLocksCountAsync();
        Task<GetLockTransactionOutput> GetLockTransactionAsync(long lockId);
        Task<GetLockListOutput> GetLocksAsync();
        Task<GetLockListOutput> GetLocksByInitiatorAsync(string initiator);
        Task<GetLockReceiverListOutput> GetLocksForReceiverAsync(string receiver);
        Task<GetLockListOutput> GetLocksByAssetAsync(string tokenSymbol);
        Task<GetRefundListOutput> GetRefundsAsync();
        Task<GetLockAssetCounterListOutput> GetAssetsCounterAsync();

    }
}