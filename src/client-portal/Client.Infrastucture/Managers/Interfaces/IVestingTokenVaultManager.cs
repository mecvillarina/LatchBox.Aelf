using AElf.Client.Dto;
using AElf.Client.LatchBox.VestingTokenVault;
using Client.Infrastructure.Models.Inputs;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IVestingTokenVaultManager : IManager
    {
        Task<TransactionResultDto> InitializeAsync();
        Task<TransactionResultDto> AddVestingAsync(AddVestingInputModel model);
        Task<TransactionResultDto> ClaimVestingAsync(long vestingId, long periodId);
        Task<TransactionResultDto> RevokeVestingAsync(long vestingId);
        Task<TransactionResultDto> ClaimRefundAsync(string tokenSymbol);

        Task<Int64Value> GetVestingsCountAsync();
        Task<GetVestingTransactionOutput> GetVestingTransactionAsync(long vestingId);
        Task<GetVestingListOutput> GetVestingsByInitiatorAsync(string initiator);
        Task<GetVestingReceiverListOutput> GetVestingsForReceiverAsync(string receiver);
        Task<GetRefundListOutput> GetRefundsAsync();
    }
}