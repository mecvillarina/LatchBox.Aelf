using AElf;
using AElf.Client.Dto;
using AElf.Client.MultiToken;
using Blazored.LocalStorage;
using Client.Infrastructure.Constants;
using Client.Infrastructure.Managers.Interfaces;
using Client.Infrastructure.Services.Interfaces;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers
{
    public class TokenManager : ManagerBase, ITokenManager
    {
        private readonly IBlockChainService _blockChainService;
        private readonly ILocalStorageService _localStorageService;
        private readonly IWalletManager _walletManager;

        public TokenManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, ILocalStorageService localStorageService, IWalletManager walletManager) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _localStorageService = localStorageService;
            _walletManager = walletManager;
        }

        public string ContactAddress => ManagerToolkit.AelfSettings.MultiTokenContractAddress;

        public async Task<TokenInfo> GetNativeTokenInfoAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            IMessage @params = new Empty { };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetNativeTokenInfo", @params);
            return TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<StringValue> GetPrimaryTokenSymbolAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            IMessage @params = new Empty { };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetPrimaryTokenSymbol", @params);
            return StringValue.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TokenInfoList> GetResourceTokenInfoListAsync()
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            IMessage @params = new Empty { };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetResourceTokenInfo", @params);
            return TokenInfoList.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TokenInfo> GetTokenInfoAsync(string symbol)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new GetTokenInfoInput()
            {
                Symbol = symbol
            };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetTokenInfo", @params);
            return TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetBalanceOutput> GetBalanceAsync(string symbol)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new GetBalanceInput
            {
                Symbol = symbol,
                Owner = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(cred.Item1.Address).Value }
            };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetBalance", @params);
            return GetBalanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TransactionResultDto> CreateAsync(string symbol, string tokenName, long totalSupply, int decimals, bool isBurnable)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new CreateInput
            {
                Symbol = symbol.ToUpper(),
                TokenName = tokenName,
                TotalSupply = totalSupply,
                Decimals = decimals,
                Issuer = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(cred.Item1.Address).Value },
                IsBurnable = isBurnable,
                IssueChainId = ManagerToolkit.AelfSettings.ChainId
            };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "Create", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> IssueAsync(string symbol, long amount, string memo, string to)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new IssueInput
            {
                Symbol = symbol.ToUpper(),
                Amount = amount,
                Memo = memo,
                To = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(to).Value },
            };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "Issue", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ApproveAsync(string spender, string symbol, long amount)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new ApproveInput
            {
                Spender = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(spender).Value },
                Symbol = symbol,
                Amount = amount,
            };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "Approve", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> UnApproveAsync(string spender, string symbol, long amount)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new UnApproveInput
            {
                Spender = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(spender).Value },
                Symbol = symbol,
                Amount = amount,
            };

            var txId = await _blockChainService.SendTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "UnApproveInput", @params);
            return await _blockChainService.CheckTransactionResultAsync(txId);
        }

        public async Task<GetAllowanceOutput> GetAllowanceAsync(string symbol, string owner, string spender)
        {
            var cred = await _walletManager.GetWalletCredentialsAsync();

            var @params = new GetAllowanceInput
            {
                Symbol = symbol,
                Owner = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(owner).Value },
                Spender = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(spender).Value },
            };

            var result = await _blockChainService.CallTransactionAsync(cred.Item1, cred.Item2, ContactAddress, "GetAllowance", @params);
            return GetAllowanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
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

    }
}
