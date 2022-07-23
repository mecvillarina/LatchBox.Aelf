using Client.App.Models;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingClaimRefundInput
    {
        public string TokenSymbol { get; set; }

        public VestingClaimRefundInput(VestingRefundModel parameter)
        {
            TokenSymbol = parameter.Refund.TokenSymbol;
        }
    }
}
