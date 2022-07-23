using System.Collections.Generic;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingReceiverListOutput
    {
        public List<VestingGetVestingTransactionForReceiverOutput> Transactions { get; set; }
    }
}
