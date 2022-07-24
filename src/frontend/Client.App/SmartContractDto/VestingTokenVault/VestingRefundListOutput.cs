using System.Collections.Generic;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingRefundListOutput
    {
        public List<VestingRefundOutput> Refunds { get; set; } = new();
    }
}
