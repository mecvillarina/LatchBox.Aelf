using AElf.Client.LatchBox.VestingTokenVault;
using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;

namespace Client.Models
{
    public class VestingAssetRefundModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public Refund Refund { get; private set; }
        public string AmountDisplay { get; private set; }
        public VestingAssetRefundModel(Refund refund)
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
