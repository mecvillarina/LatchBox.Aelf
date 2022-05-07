using AElf.Client.LatchBox.MultiCrowdSale;
using MudBlazor;

namespace Client.Infrastructure.Models
{
    public class ActiveCrowdSaleModel
    {
        public CrowdSale CrowdSale { get; }
        public long RaisedAmount { get; }
        public string TokenName { get; }
        public string TokenSymbol { get; }
        public int TokenDecimals { get; }
        public string Status { get; set; }
        public Color StatusColor { get; set; }
        public bool CanBuy { get; set; }

        public ActiveCrowdSaleModel(CrowdSaleOutput output, WalletInformation loggedWallet = null)
        {
            CrowdSale = output.CrowdSale;
            RaisedAmount = output.RaisedAmount;
            TokenName = output.TokenName;
            TokenSymbol = output.TokenSymbol;
            TokenDecimals = output.TokenDecimals;

            if (CrowdSale.IsActive)
            {
                if (CrowdSale.SaleEndDate.ToDateTimeOffset() > System.DateTimeOffset.UtcNow && CrowdSale.HardCapNativeTokenAmount != RaisedAmount)
                {
                    Status = "ON SALE";
                    StatusColor = Color.Primary;
                    //CanBuy = loggedWallet != null && loggedWallet.Address != CrowdSale.Initiator.Value.ToString();
                    CanBuy = true;
                }
                else if (CrowdSale.HardCapNativeTokenAmount == RaisedAmount)
                {
                    Status = "FULL";
                    StatusColor = Color.Info;
                }
            }
           
        }
    }
}
