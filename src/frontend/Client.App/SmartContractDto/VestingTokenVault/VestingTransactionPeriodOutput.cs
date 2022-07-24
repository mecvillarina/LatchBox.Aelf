using System.Collections.Generic;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingTransactionPeriodOutput
    {
        public VestingPeriodOutput Period { get; set; }
        public List<VestingReceiverOutput> Receivers { get; set; } = new();
    }
}
