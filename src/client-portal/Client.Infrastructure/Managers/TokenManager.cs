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
        private readonly IBlockchainManager _blockchainManager;

        public TokenManager(IManagerToolkit managerToolkit, IBlockChainService blockChainService, ILocalStorageService localStorageService, IWalletManager walletManager, IBlockchainManager blockchainManager) : base(managerToolkit)
        {
            _blockChainService = blockChainService;
            _localStorageService = localStorageService;
            _walletManager = walletManager;
            _blockchainManager = blockchainManager;
        }

        public string MainChainContactAddress => _blockchainManager.FetchMainChainTokenAddress();
        public string SideChainContactAddress => _blockchainManager.FetchSideChainTokenAddress();

        public async Task<TokenInfo> GetNativeTokenInfoOnMainChainAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchMainChainStatus();

            IMessage @params = new Empty { };

            var result = await _blockChainService.CallMainChainTransactionAsync(keyPair, MainChainContactAddress, "GetNativeTokenInfo", @params, chainStatus);
            return TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TokenInfo> GetNativeTokenInfoOnSideChainAsync()
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            IMessage @params = new Empty { };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, SideChainContactAddress, "GetNativeTokenInfo", @params, chainStatus);
            return TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TokenInfo> GetTokenInfoOnMainChainAsync(string symbol)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchMainChainStatus();

            var @params = new GetTokenInfoInput()
            {
                Symbol = symbol
            };

            var result = await _blockChainService.CallMainChainTransactionAsync(keyPair, MainChainContactAddress, "GetTokenInfo", @params, chainStatus);
            return TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TokenInfo> GetTokenInfoOnSideChainAsync(string symbol)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new GetTokenInfoInput()
            {
                Symbol = symbol
            };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, SideChainContactAddress, "GetTokenInfo", @params, chainStatus);
            return TokenInfo.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetBalanceOutput> GetBalanceOnMainChainAsync(string symbol)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var address = await _walletManager.GetWalletAddressAsync();
            var chainStatus = _blockchainManager.FetchMainChainStatus();

            var val = ChainHelper.ConvertBase58ToChainId(chainStatus.ChainId);

            var @params = new GetBalanceInput
            {
                Symbol = symbol,
                Owner = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(address).Value }
            };

            var result = await _blockChainService.CallMainChainTransactionAsync(keyPair, MainChainContactAddress, "GetBalance", @params, chainStatus);
            return GetBalanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<GetBalanceOutput> GetBalanceOnSideChainAsync(string symbol)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var address = await _walletManager.GetWalletAddressAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var val = ChainHelper.ConvertBase58ToChainId(chainStatus.ChainId);

            var @params = new GetBalanceInput
            {
                Symbol = symbol,
                Owner = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(address).Value }
            };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, SideChainContactAddress, "GetBalance", @params, chainStatus);
            return GetBalanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task<TransactionResultDto> CreateAsync(string symbol, string tokenName, long totalSupply, int decimals, bool isBurnable)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var address = await _walletManager.GetWalletAddressAsync();

            var issueChainId = _blockchainManager.GetSideChainId();

            var @params = new CreateInput
            {
                Symbol = symbol.ToUpper(),
                TokenName = tokenName,
                TotalSupply = totalSupply,
                Decimals = decimals,
                Issuer = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(address).Value },
                IsBurnable = isBurnable,
                IssueChainId = issueChainId
            };

            var tx = await _blockChainService.SendMainChainTransactionAsync(keyPair, MainChainContactAddress, "Create", @params);
            return await _blockChainService.CheckMainChainTransactionResultAsync(tx.Item1);
        }

        public async Task<TransactionResultDto> IssueOnMainChainAsync(string symbol, long amount, string memo, string to)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new IssueInput
            {
                Symbol = symbol.ToUpper(),
                Amount = amount,
                Memo = memo,
                To = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(to).Value },
            };

            var txId = await _blockChainService.SendMainChainTransactionAsync(keyPair, MainChainContactAddress, "Issue", @params);
            return await _blockChainService.CheckMainChainTransactionResultAsync(txId.Item1);
        }

        public async Task<TransactionResultDto> IssueOnSideChainAsync(string symbol, long amount, string memo, string to)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new IssueInput
            {
                Symbol = symbol.ToUpper(),
                Amount = amount,
                Memo = memo,
                To = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(to).Value },
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, SideChainContactAddress, "Issue", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> ApproveAsync(string spender, string symbol, long amount)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new ApproveInput
            {
                Spender = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(spender).Value },
                Symbol = symbol,
                Amount = amount,
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, SideChainContactAddress, "Approve", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<TransactionResultDto> UnApproveAsync(string spender, string symbol, long amount)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var @params = new UnApproveInput
            {
                Spender = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(spender).Value },
                Symbol = symbol,
                Amount = amount,
            };

            var txId = await _blockChainService.SendSideChainTransactionAsync(keyPair, SideChainContactAddress, "UnApproveInput", @params);
            return await _blockChainService.CheckSideChainTransactionResultAsync(txId);
        }

        public async Task<GetAllowanceOutput> GetAllowanceAsync(string symbol, string owner, string spender)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();
            var chainStatus = _blockchainManager.FetchSideChainStatus();

            var @params = new GetAllowanceInput
            {
                Symbol = symbol,
                Owner = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(owner).Value },
                Spender = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(spender).Value },
            };

            var result = await _blockChainService.CallSideChainTransactionAsync(keyPair, SideChainContactAddress, "GetAllowance", @params, chainStatus).ConfigureAwait(false);
            return GetAllowanceOutput.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));
        }

        public async Task AddToTokenSymbolsStorageAsync(string symbol)
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

        public async Task RemoveFromTokenSymbolsStorageAsync(string symbol)
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

        public async Task CacheTokenInfoAsync(int chainId, TokenInfoBase tokenInfo)
        {
            await _localStorageService.SetItemAsync($"{chainId}_{tokenInfo.Symbol}", tokenInfo);
        }

        public async Task<TokenInfoBase> GetCacheTokenInfoAsync(int chainId, string symbol)
        {
            var isExists = await _localStorageService.ContainKeyAsync($"{chainId}_{symbol}");
            if (isExists)
            {
                return await _localStorageService.GetItemAsync<TokenInfoBase>($"{chainId}_{symbol}");
            }

            return null;
        }

        public async Task<TransactionResultDto> CreateSideChainTokenAsync(TokenInfo tokenInfo)
        {
            var keyPair = await _walletManager.GetWalletKeyPairAsync();

            var validateParams = new ValidateTokenInfoExistsInput
            {
                Symbol = tokenInfo.Symbol,
                TokenName = tokenInfo.TokenName,
                Decimals = tokenInfo.Decimals,
                IsBurnable = tokenInfo.IsBurnable,
                IssueChainId = tokenInfo.IssueChainId,
                Issuer = tokenInfo.Issuer,
                TotalSupply = tokenInfo.TotalSupply
            };

            var validateTx = await _blockChainService.SendMainChainTransactionAsync(keyPair, SideChainContactAddress, "ValidateTokenInfoExists", validateParams);
            var validateTxResult = await _blockChainService.CheckMainChainTransactionResultAsync(validateTx.Item1);

            if (validateTxResult.Status == TransactionResultStatus.Mined.ToString().ToUpper())
            {
                while (true)
                {
                    var chainStatus = _blockchainManager.FetchMainChainStatus();
                    if ((chainStatus.LastIrreversibleBlockHeight - validateTxResult.BlockNumber) > 80)
                        break;

                    await Task.Delay(15000);
                }

                await Task.Delay(3000);

                var mainChainId = await _blockChainService.GetMainChainIdAsync();
                var merklePath = await _blockChainService.GetMainChainMerklePathByTransactionIdAsync(validateTx.Item1);
                var createTokenParams = new CrossChainCreateTokenInput
                {
                    FromChainId = mainChainId,
                    ParentChainHeight = validateTxResult.BlockNumber,
                    TransactionBytes = ByteString.CopyFrom(ByteArrayHelper.HexStringToByteArray(validateTx.Item2)),
                    MerklePath = new AElf.Client.Proto.MerklePath()
                };

                foreach (var node in merklePath.MerklePathNodes)
                {
                    createTokenParams.MerklePath.MerklePathNodes.Add(new AElf.Client.Proto.MerklePathNode()
                    {
                        Hash = new AElf.Client.Proto.Hash() { Value = AElf.Types.Hash.LoadFromHex(node.Hash).Value },
                        IsLeftChildNode = node.IsLeftChildNode
                    });
                }

                var createTokenTxId = await _blockChainService.SendSideChainTransactionAsync(keyPair, SideChainContactAddress, "CrossChainCreateToken", createTokenParams);
                var createTokenTxResult = await _blockChainService.CheckSideChainTransactionResultAsync(createTokenTxId);
                return createTokenTxResult;
            }

            return validateTxResult;
        }

        //public async Task<Address> GetCrossChainTransferTokenContractAddress(int chainId)
        //{
        //    var keyPair = await _walletManager.GetWalletKeyPairAsync();

        //    var @params = new GetCrossChainTransferTokenContractAddressInput
        //    {
        //       ChainId = chainId
        //    };

        //    var result = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "GetCrossChainTransferTokenContractAddress", @params);
        //    return Address.Parser.ParseFrom(ByteArrayHelper.HexStringToByteArray(result));

        //}
        //public async Task<TransactionResultDto> CrossChainTransferAsync(string to, string symbol, long amount, string memo, int toChainId)
        //{
        //    var keyPair = await _walletManager.GetWalletKeyPairAsync();

        //    var chainId = await _blockChainService.GetChainIdAsync();
        //    var cA = await GetCrossChainTransferTokenContractAddress(chainId);
        //    var @params = new CrossChainTransferInput
        //    {
        //        To = new AElf.Client.Proto.Address { Value = AElf.Types.Address.FromBase58(to).Value },
        //        Symbol = symbol.ToUpper(),
        //        Amount = amount,
        //        Memo = memo,
        //        ToChainId = toChainId,
        //        IssueChainId = chainId
        //    };

        //    var crossChainTransferTxId = await _blockChainService.SendTransactionAsync(keyPair, ContactAddress, "CrossChainTransfer", @params);
        //    var crossChainTransferTxResult = await _blockChainService.CheckTransactionResultAsync(crossChainTransferTxId);

        //    if (crossChainTransferTxResult.Status == "MINED")
        //    {
        //        var merklePath = await _blockChainService.GetMerklePathByTransactionIdAsync(crossChainTransferTxId);
        //        var generateRawTransaction = await _blockChainService.GenerateRawTransactionAsync(keyPair, ContactAddress, "CrossChainTransfer", @params);

        //        var params2 = new CrossChainReceiveTokenInput
        //        {
        //            FromChainId = chainId,
        //            ParentChainHeight = crossChainTransferTxResult.BlockNumber,
        //            TransferTransactionBytes = ByteString.CopyFrom(ByteArrayHelper.HexStringToByteArray(generateRawTransaction)),
        //            MerklePath = new MerklePath()
        //        };

        //        foreach (var node in merklePath.MerklePathNodes)
        //        {
        //            params2.MerklePath.MerklePathNodes.Add(new MerklePathNode()
        //            {
        //                Hash = new AElf.Client.Proto.Hash() { Value =  AElf.Types.Hash.LoadFromHex(node.Hash).Value },
        //                IsLeftChildNode = node.IsLeftChildNode
        //            });
        //        }

        //        var crossChainReceiveTxId = await _blockChainService.CallTransactionAsync(keyPair, ContactAddress, "CrossChainReceiveToken", params2);
        //        //var crossChainReceiveTxResult = await _blockChainService.CheckTransactionResultAsync2(crossChainReceiveTxId);
        //        //return crossChainReceiveTxResult;
        //    }

        //    return crossChainTransferTxResult;
        //}
    }
}
