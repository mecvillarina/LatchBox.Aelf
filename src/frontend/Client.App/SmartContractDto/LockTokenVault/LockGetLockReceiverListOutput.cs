using System.Collections.Generic;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockGetLockReceiverListOutput
    {
        public List<LockGetLockTransactionForReceiverOutput> LockTransactions { get; set; }
    }
}
