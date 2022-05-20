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
        Task<TokenInfo> GetNativeTokenInfoAsync();
        Task<StringValue> GetPrimaryTokenSymbolAsync();
        Task<TokenInfoList> GetResourceTokenInfoListAsync();
        Task<TokenInfo> GetTokenInfoAsync(string symbol);
        Task<GetBalanceOutput> GetBalanceAsync(string symbol);
        Task<TransactionResultDto> CreateAsync(string symbol, string tokenName, long totalSupply, int decimals, bool isBurnable);
        Task<TransactionResultDto> IssueAsync(string symbol, long amount, string memo, string to);
        Task<TransactionResultDto> ApproveAsync(string spender, string symbol, long amount);
        Task<TransactionResultDto> UnApproveAsync(string spender, string symbol, long amount);
        Task<GetAllowanceOutput> GetAllowanceAsync(string symbol, string owner, string spender);
        //Task<TransactionResultDto> CrossChainTransferAsync(string to, string symbol, long amount, string memo, int toChainId);

        Task AddTokenSymbolToStorageAsync(string symbol);
        Task<List<string>> GetTokenSymbolsFromStorageAsync();
        Task RemoveTokenSymbolFromStorageAsync(string symbol);
    }
}