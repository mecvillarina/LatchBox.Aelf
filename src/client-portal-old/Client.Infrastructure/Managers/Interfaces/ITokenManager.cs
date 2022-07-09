using AElf.Client.Dto;
using AElf.Client.MultiToken;
using Client.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface ITokenManager : IManager
    {
        string SideChainContactAddress { get; }
        Task<TokenInfo> GetNativeTokenInfoOnMainChainAsync();
        Task<TokenInfo> GetNativeTokenInfoOnSideChainAsync();
        Task<TokenInfo> GetTokenInfoOnMainChainAsync(string symbol);
        Task<TokenInfo> GetTokenInfoOnSideChainAsync(string symbol);
        Task<GetBalanceOutput> GetBalanceOnMainChainAsync(string symbol);
        Task<GetBalanceOutput> GetBalanceOnSideChainAsync(string symbol);
        Task<TransactionResultDto> CreateAsync(string symbol, string tokenName, long totalSupply, int decimals, bool isBurnable);
        Task<TransactionResultDto> IssueOnMainChainAsync(string symbol, long amount, string memo, string to);
        Task<TransactionResultDto> IssueOnSideChainAsync(string symbol, long amount, string memo, string to);
        Task<TransactionResultDto> ApproveAsync(string spender, string symbol, long amount);
        Task<TransactionResultDto> UnApproveAsync(string spender, string symbol, long amount);
        Task<GetAllowanceOutput> GetAllowanceAsync(string symbol, string owner, string spender);
        Task<TransactionResultDto> CreateSideChainTokenAsync(TokenInfo tokenInfo);
        //Task<TransactionResultDto> CrossChainTransferAsync(string to, string symbol, long amount, string memo, int toChainId);

        Task AddToTokenSymbolsStorageAsync(string symbol);
        Task<List<string>> GetTokenSymbolsFromStorageAsync();
        Task RemoveFromTokenSymbolsStorageAsync(string symbol);

        Task CacheTokenInfoAsync(int chainId, TokenInfoBase tokenInfo);
        Task<TokenInfoBase> GetCacheTokenInfoAsync(int chainId, string symbol);
    }
}