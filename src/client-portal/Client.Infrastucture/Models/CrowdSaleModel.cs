using AElf.Client.LatchBox.MultiCrowdSale;
using Humanizer;
using MudBlazor;
using System;

namespace Client.Infrastructure.Models
{
    public class CrowdSaleModel
    {
        public CrowdSale CrowdSale { get; }
        public long RaisedAmount { get; }
        public string TokenName { get; }
        public string TokenSymbol { get; }
        public int TokenDecimals { get; }
        public string Status { get; set; }
        public string Substatus { get; set; }
        public Color StatusColor { get; set; }
        public bool CanBuy { get; set; }

        public CrowdSaleModel(CrowdSaleOutput output)
        {
            CrowdSale = output.CrowdSale;
            RaisedAmount = output.RaisedAmount;
            TokenName = output.TokenName;
            TokenSymbol = output.TokenSymbol;
            TokenDecimals = output.TokenDecimals;

            if (CrowdSale.IsActive)
            {
                if (CrowdSale.SaleStartDate.ToDateTimeOffset() > System.DateTimeOffset.UtcNow)
                {
                    Status = "UPCOMING";
                    StatusColor = Color.Info;
                }

                if (CrowdSale.SaleEndDate.ToDateTimeOffset() < System.DateTimeOffset.UtcNow)
                {
                    Status = "ENDED";
                    StatusColor = Color.Success;
                }

                if (CrowdSale.SaleStartDate.ToDateTimeOffset() < System.DateTimeOffset.UtcNow && CrowdSale.SaleEndDate.ToDateTimeOffset() > System.DateTimeOffset.UtcNow)
                {
                    Status = "ONGOING";
                    Substatus = $"{Environment.NewLine} {CrowdSale.SaleEndDate.ToDateTimeOffset().Subtract(System.DateTimeOffset.UtcNow).Humanize(3)}";
                }
            }
            else
            {
                if (!CrowdSale.IsCancelled)
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
