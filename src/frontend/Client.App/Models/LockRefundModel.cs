using Application.Common.Extensions;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.LockTokenVault;

namespace Client.App.Models
{
    public class LockRefundModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public LockRefundOutput Refund { get; private set; }
        public string AmountDisplay { get; private set; }
        public LockRefundModel(LockRefundOutput refund)
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
