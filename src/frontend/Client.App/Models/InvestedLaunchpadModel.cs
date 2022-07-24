using Client.App.SmartContractDto.Launchpad;
using MudBlazor;

namespace Client.App.Models
{
    public class InvestedLaunchpadModel
    {
        public CrowdSale Launchpad { get; }
        public CrowdSaleInvestment Investment { get; }
        public string TokenName { get; }
        public string TokenSymbol { get; }
        public int TokenDecimals { get; }
        public string Status { get; set; }
        public Color StatusColor { get; set; }
        public long? LockId { get; set; }
        public long InvestedAmount { get; }
        public InvestedLaunchpadModel(CrowdSaleInvestmentOutput output)
        {
            Launchpad = output.CrowdSaleOutput.CrowdSale;
            TokenName = output.CrowdSaleOutput.TokenName;
            TokenSymbol = output.CrowdSaleOutput.TokenSymbol;
            TokenDecimals = output.CrowdSaleOutput.TokenDecimals;
            Investment = output.Investment;
            InvestedAmount = Investment.TokenAmount;
            LockId = Launchpad.LockId > 0 ? Launchpad.LockId : null;

            if (Launchpad.IsActive)
            {
                if (Launchpad.SaleStartDate.GetUniversalDateTime() > System.DateTimeOffset.UtcNow)
                {
                    Status = "UPCOMING";
                    StatusColor = Color.Primary;
                }
                else if (Launchpad.SaleEndDate.GetUniversalDateTime() > System.DateTimeOffset.UtcNow)
                {
                    Status = "ONGOING";
                    StatusColor = Color.Primary;
                }
                else
                {
                    Status = "WAITING TO BE COMPLETED";
                    StatusColor = Color.Info;
                }
            }
            else
            {
                if (!Launchpad.IsCancelled)
                {
                    if (Launchpad.IsSuccess)
                    {
                        Status = "ENDED";
                        StatusColor = Color.Info;
                    }
                    else
                    {
                        Status = "GOAL NOT MET & REFUNDED";
                        StatusColor = Color.Error;
                    }
                }
                else
                {
                    Status = "CANCELLED & REFUNDED";
                    StatusColor = Color.Error;
                }
            }
        }
    }
}
