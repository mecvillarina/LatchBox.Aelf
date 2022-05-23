using AElf.Client.LatchBox.MultiCrowdSale;
using Humanizer;
using MudBlazor;
using System;

namespace Client.Infrastructure.Models
{
    public class LaunchpadModel
    {
        public CrowdSale Launchpad { get; }
        public long RaisedAmount { get; }
        public string TokenName { get; }
        public string TokenSymbol { get; }
        public int TokenDecimals { get; }
        public string Status { get; set; }
        public string Substatus { get; set; }
        public Color StatusColor { get; set; }
        public bool CanBuy { get; set; }

        public LaunchpadModel(CrowdSaleOutput output)
        {
            Launchpad = output.CrowdSale;
            RaisedAmount = output.RaisedAmount;
            TokenName = output.TokenName;
            TokenSymbol = output.TokenSymbol;
            TokenDecimals = output.TokenDecimals;

            if (Launchpad.IsActive)
            {
                if (Launchpad.SaleStartDate.ToDateTimeOffset() > System.DateTimeOffset.UtcNow)
                {
                    Status = "UPCOMING";
                    StatusColor = Color.Info;
                }
                else if(Launchpad.SaleEndDate.ToDateTimeOffset() < System.DateTimeOffset.UtcNow)
                {
                    Status = "ENDED";
                    StatusColor = Color.Success;
                }

                if (Launchpad.SaleStartDate.ToDateTimeOffset() < System.DateTimeOffset.UtcNow && Launchpad.SaleEndDate.ToDateTimeOffset() > System.DateTimeOffset.UtcNow)
                {
                    Status = "ONGOING";
                    CanBuy = true;
                    StatusColor = Color.Primary;
                    Substatus = $"{Environment.NewLine} {Launchpad.SaleEndDate.ToDateTimeOffset().Subtract(System.DateTimeOffset.UtcNow).Humanize(3)}";
                }
            }
            else
            {
                if (!Launchpad.IsCancelled)
                {
                    Status = "ENDED";
                    StatusColor = Color.Success;
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
