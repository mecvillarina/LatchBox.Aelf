using System.Collections.Generic;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockReceiverListOutput
    {
        public List<LockGetLockTransactionForReceiverOutput> LockTransactions { get; set; }
    }
}
