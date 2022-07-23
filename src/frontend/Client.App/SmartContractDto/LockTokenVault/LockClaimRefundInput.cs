using Client.App.Models;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockClaimRefundInput
    {
        public string TokenSymbol { get; set; }

        public LockClaimRefundInput(LockRefundModel parameter)
        {
            TokenSymbol = parameter.Refund.TokenSymbol;
        }
    }
}
