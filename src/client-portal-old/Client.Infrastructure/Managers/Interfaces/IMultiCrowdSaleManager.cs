using AElf.Client.Dto;
using AElf.Client.LatchBox.MultiCrowdSale;
using Client.Infrastructure.Models.Inputs;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IMultiCrowdSaleManager : IManager
    {
        string ContactAddress { get; }
        Task<TransactionResultDto> InitializeAsync();
        Task<TransactionResultDto> CreateAsync(CreateLaunchpadInputModel model);
        Task<TransactionResultDto> CancelAsync(long crowdSaleId);
        Task<TransactionResultDto> InvestAsync(long crowdSaleId, long amount);
        Task<TransactionResultDto> CompleteAsync(long crowdSaleId);
        Task<TransactionResultDto> UpdateLockInfoAsync(long crowdSaleId, long lockId);
        //Views
        Task<Int64Value> GetCrowdSaleCountAsync();
        Task<Int64Value> GetUpcomingCrowdSaleCountAsync();
        Task<Int64Value> GetOngoingCrowdSaleCountAsync();
        Task<CrowdSaleOutput> GetCrowdSaleAsync(long crowdSaleId);
        Task<CrowdSaleListOutput> GetCrowdSalesByInitiatorAsync(string initiator);
        Task<CrowdSaleListOutput> GetCrowdSalesAsync(bool isUpcoming, bool isOngoing);
        Task<CrowdSaleInvestmentListOutput> GetCrowdSaleInvestments(long crowdSaleId);
        Task<CrowdSaleByInvestorListOuput> GetCrowdSalesByInvestorAsync(string investor);
    }
}