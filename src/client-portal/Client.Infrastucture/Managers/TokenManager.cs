using AElf;
using AElf.Client.Dto;
using AElf.Client.MultiToken;
using AElf.Types;
using Blazored.LocalStorage;
using Client.Infrastructure.Constants;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Models;
using Client.Infrastructure.Services.Interfaces;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class TokenManager : ManagerBase, ITokenManager
    {
        private readonly IBlockChainService _blockChainService;
        private readonly ILocalStorageService _localStorageService;

        public TokenManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, ILocalStorageService localStorageService) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _localStorageService = localStorageService;
        }

        public async Task<TokenInfo> GetNativeTokenInfoAsync(WalletInformation wallet, string password)
        {
            IMessage @params = new Empty { };

            var result = await _blockChainService.ExecuteTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "GetNativeTokenInfo", @params);
            return TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<StringValue> GetPrimaryTokenSymbolAsync(WalletInformation wallet, string password)
        {
            IMessage @params = new Empty { };

            var result = await _blockChainService.ExecuteTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "GetPrimaryTokenSymbol", @params);
            return StringValue.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TokenInfoList> GetResourceTokenInfoListAsync(WalletInformation wallet, string password)
        {
            IMessage @params = new Empty { };

            var result = await _blockChainService.ExecuteTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "GetResourceTokenInfo", @params);
            return TokenInfoList.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TokenInfo> GetTokenInfoAsync(WalletInformation wallet, string password, string symbol)
        {
            var paramGetTokenInfo = new GetTokenInfoInput()
            {
                Symbol = symbol
            };

            var result = await _blockChainService.ExecuteTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "GetTokenInfo", paramGetTokenInfo);
            return TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetBalanceOutput> GetBalanceAsync(WalletInformation wallet, string password, string symbol)
        {
            var paramGetBalance = new GetBalanceInput
            {
                Symbol = symbol,
                Owner = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(wallet.Address).Value }
            };

            var result = await _blockChainService.ExecuteTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "GetBalance", paramGetBalance);
            return GetBalanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TransactionResultDto> CreateTokenAsync(WalletInformation wallet, string password, string symbol, string tokenName, long totalSupply, int decimals, bool isBurnable)
        {
            var paramCreateInput = new CreateInput
            {
                Symbol = symbol.ToUpper(),
                TokenName = tokenName,
                TotalSupply = totalSupply,
                Decimals = decimals,
                Issuer = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(wallet.Address).Value },
                IsBurnable = isBurnable,
                IssueChainId = ManagerToolkit.AelfSettings.ChainId
            };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "Create", paramCreateInput);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task AddTokenSymbolToStorageAsync(string symbol)
        {
            symbol = symbol.ToUpper();
            var tokenSymbolList = await _localStorageService.GetItemAsync<List<string>>(StorageConstants.Local.TokenSymbols);
            tokenSymbolList ??= new List<string>();

            if (!tokenSymbolList.Any(x => x == symbol))
            {
                tokenSymbolList.Add(symbol);
                await _localStorageService.SetItemAsync(StorageConstants.Local.TokenSymbols, tokenSymbolList);
            }
        }

        public async Task<List<string>> GetTokenSymbolsFromStorageAsync()
        {
            var tokenSymbolList = await _localStorageService.GetItemAsync<List<string>>(StorageConstants.Local.TokenSymbols);
            tokenSymbolList ??= new List<string>();
            return tokenSymbolList;
        }

        public async Task RemoveTokenSymbolFromStorageAsync(string symbol)
        {
            symbol = symbol.ToUpper();
            var tokenSymbolList = await _localStorageService.GetItemAsync<List<string>>(StorageConstants.Local.TokenSymbols);
            tokenSymbolList ??= new List<string>();

            if (tokenSymbolList.Any(x => x == symbol))
            {
                tokenSymbolList.Remove(symbol);
                await _localStorageService.SetItemAsync(StorageConstants.Local.TokenSymbols, tokenSymbolList);
            }
        }

        public async Task<TransactionResultDto> IssueTokenAsync(WalletInformation wallet, string password, string symbol, long amount, string memo, string to)
        {
            var paramCreateInput = new IssueInput
            {
                Symbol = symbol.ToUpper(),
                Amount = amount,
                Memo = memo,
                To = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(to).Value },
            };

            var txId = await _blockChainService.SendTransactionAsync(wallet, password, ManagerToolkit.AelfSettings.MultiTokenContractAddress, "Issue", paramCreateInput);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }
    }
}
