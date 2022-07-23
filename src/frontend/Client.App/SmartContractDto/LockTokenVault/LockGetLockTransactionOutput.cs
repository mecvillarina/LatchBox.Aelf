using System.Collections.Generic;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockGetLockTransactionOutput
    {
        public LockOutput Lock { get; set; }
        public List<LockReceiverOutput> Receivers { get; set; }
    }
}
