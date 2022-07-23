using Application.Common.Extensions;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.VestingTokenVault;

namespace Client.App.Models
{
    public class VestingRefundModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public VestingRefundOutput Refund { get; private set; }
        public string AmountDisplay { get; private set; }
        public VestingRefundModel(VestingRefundOutput refund)
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
