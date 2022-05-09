using AElf.Client.LatchBox.LockTokenVault;
using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;

namespace Client.Models
{
    public class AssetRefundModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public Refund Refund { get; private set; }
        public string AmountDisplay { get; private set; }
        public AssetRefundModel(Refund refund)
        {
            Refund = refund;
        }

        public void SetTokenInfo(TokenInfo tokenInfo)
        {
            TokenInfo = tokenInfo;
            AmountDisplay = $"{Refund.Amount.ToAmount(TokenInfo.Decimals).ToAmountDisplay(TokenInfo.Decimals)} {TokenInfo.Symbol}";
        }
    }
}
