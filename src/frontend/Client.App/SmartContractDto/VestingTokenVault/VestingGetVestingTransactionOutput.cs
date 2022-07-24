using System.Collections.Generic;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingGetVestingTransactionOutput
    {
        public VestingOutput Vesting { get; set; }
        public List<VestingTransactionPeriodOutput> Periods { get; set; } = new();
    }
}
