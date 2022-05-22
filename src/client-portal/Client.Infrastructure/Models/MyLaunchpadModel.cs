using AElf.Client.LatchBox.MultiCrowdSale;
using MudBlazor;

namespace Client.Infrastructure.Models
{
    public class MyLaunchpadModel
    {
        public CrowdSale Launchpad { get; }
        public long RaisedAmount { get; }
        public string TokenName { get; }
        public string TokenSymbol { get; }
        public int TokenDecimals { get; }
        public string Status { get; set; }
        public Color StatusColor { get; set; }
        public bool CanCancel { get; set; }
        public bool CanComplete { get; set; }
        public bool CanRefund { get; set; }

        public MyLaunchpadModel(CrowdSaleOutput output)
        {
            Launchpad = output.CrowdSale;
            RaisedAmount = output.RaisedAmount;
            TokenName = output.TokenName;
            TokenSymbol = output.TokenSymbol;
            TokenDecimals = output.TokenDecimals;

            if (Launchpad.IsActive)
            {
                CanCancel = true;

                if (Launchpad.SaleStartDate.ToDateTimeOffset() > System.DateTimeOffset.UtcNow)
                {
                    Status = "UPCOMING";
                    StatusColor = Color.Primary;
                }
                else if (Launchpad.SaleEndDate.ToDateTimeOffset() > System.DateTimeOffset.UtcNow)
                {
                    Status = "ONGOING";
                    StatusColor = Color.Primary;
                }
                else
                {
                    if (Launchpad.HardCapNativeTokenAmount == RaisedAmount || RaisedAmount >= Launchpad.SoftCapNativeTokenAmount)
                    {
                        CanComplete = true;
                        Status = "NEED ACTION: COMPLETE";
                        StatusColor = Color.Info;
                    }
                    else
                    {
                        CanRefund = true;
                        Status = "NEED ACTION: REFUND";
                        StatusColor = Color.Error;
                    }
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
                        Status = "GOAL NOT MET";
                        StatusColor = Color.Error;
                    }
                }
                else
                {
                    Status = "CANCELLED";
                    StatusColor = Color.Error;
                }
            }
        }
    }
}
