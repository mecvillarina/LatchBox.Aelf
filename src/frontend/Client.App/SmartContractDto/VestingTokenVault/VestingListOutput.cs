using System.Collections.Generic;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingListOutput
    {
        public List<VestingGetVestingOutput> Transactions { get; set; }
    }
}
