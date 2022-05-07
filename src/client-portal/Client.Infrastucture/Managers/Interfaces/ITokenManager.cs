using AElf.Client.Dto;
using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface ITokenManager : IManager
    {
        string ContactAddress { get; }
        Task<TokenInfo> GetNativeTokenInfoAsync(WalletInformation wallet, string password);
        Task<StringValue> GetPrimaryTokenSymbolAsync(WalletInformation wallet, string password);
        Task<TokenInfoList> GetResourceTokenInfoListAsync(WalletInformation wallet, string password);
        Task<TokenInfo> GetTokenInfoAsync(WalletInformation wallet, string password, string symbol);
        Task<GetBalanceOutput> GetBalanceAsync(WalletInformation wallet, string password, string symbol);
        Task<TransactionResultDto> CreateAsync(WalletInformation wallet, string password, string symbol, string tokenName, long totalSupply, int decimals, bool isBurnable);
        Task<TransactionResultDto> IssueAsync(WalletInformation wallet, string password, string symbol, long amount, string memo, string to);
        Task<TransactionResultDto> ApproveAsync(WalletInformation wallet, string password, string spender, string symbol, long amount);
        Task<GetAllowanceOutput> GetAllowanceAsync(WalletInformation wallet, string password, string symbol, string owner, string spender);

        Task AddTokenSymbolToStorageAsync(string symbol);
        Task<List<string>> GetTokenSymbolsFromStorageAsync();
        Task RemoveTokenSymbolFromStorageAsync(string symbol);
    }
}