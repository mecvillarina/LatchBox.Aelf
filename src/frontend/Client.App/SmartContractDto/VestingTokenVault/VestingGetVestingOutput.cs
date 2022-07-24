using System.Collections.Generic;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingGetVestingOutput
    {
        public VestingOutput Vesting { get; set; }
        public List<VestingPeriodOutput> Periods { get; set; } = new();
    }
}
