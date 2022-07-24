using Application.Common.Dtos;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.Launchpad;
using Client.App.SmartContractDto.LockTokenVault;
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
    }
}
