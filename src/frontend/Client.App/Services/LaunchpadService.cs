using Application.Common.Dtos;
using Client.App.SmartContractDto.Launchpad;
using Client.App.SmartContractDto.VestingTokenVault;
using System.Threading.Tasks;

namespace Client.App.Services
{
    public class LaunchpadService
    {
        private readonly NightElfService _nightElfService;
        private readonly ChainService _chainService;

        public LaunchpadService(NightElfService nightElfService, ChainService chainService)
        {
            _nightElfService = nightElfService;
            _chainService = chainService;
        }

        public async Task<CrowdSaleListOutput> GetCrowdSalesAsync(CrowdSaleGetCrowdSalesInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.CallTx<CrowdSaleListOutput>(chain.LaunchpadContractAddress, "GetCrowdSales", input);
        }

        public async Task<CrowdSaleOutput> GetCrowdSaleAsync(CrowdSaleGetCrowdSaleInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.CallTx<CrowdSaleOutput>(chain.LaunchpadContractAddress, "GetCrowdSale", input);
        }

        public async Task<CrowdSaleInvestmentListOutput> GetCrowdSaleInvestmentsAsync(CrowdSaleGetCrowdSaleInvestorsInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.CallTx<CrowdSaleInvestmentListOutput>(chain.LaunchpadContractAddress, "GetCrowdSaleInvestments", input);
        }

        public async Task<CrowdSaleListOutput> GetCrowdSalesByInitiatorAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            var walletAddress = await _nightElfService.GetAddressAsync();
            return await _nightElfService.CallTx<CrowdSaleListOutput>(chain.LaunchpadContractAddress, "GetCrowdSalesByInitiator", walletAddress);
        }
        public async Task<CrowdSaleByInvestorListOutput> GetCrowdSalesByInvestorAsync()
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            var walletAddress = await _nightElfService.GetAddressAsync();
            return await _nightElfService.CallTx<CrowdSaleByInvestorListOutput>(chain.LaunchpadContractAddress, "GetCrowdSalesByInvestor", walletAddress);
        }

        public async Task<TransactionResultDto> CreateAsync(CrowdSaleCreateInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.LaunchpadContractAddress, "Create", input);
        }

        public async Task<TransactionResultDto> CancelAsync(CrowdSaleCancelInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.LaunchpadContractAddress, "Cancel", input);
        }

        public async Task<TransactionResultDto> InvestAsync(CrowdSaleInvestInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.LaunchpadContractAddress, "Invest", input);
        }

        public async Task<TransactionResultDto> CompleteAsync(CrowdSaleCompleteInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.LaunchpadContractAddress, "Complete", input);
        }

        public async Task<TransactionResultDto> UpdateLockInfoAsync(CrowdSaleUpdateLockInfoInput input)
        {
            var chain = await _chainService.FetchCurrentChainInfoAsync();
            return await _nightElfService.SendTxAsync(chain.LaunchpadContractAddress, "UpdateLockInfo", input);
        }
    }
}
