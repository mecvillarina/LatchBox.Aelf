using System.Collections.Generic;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class GetLockReceiverListOutput
    {
        public List<GetLockTransactionForReceiverOutput> LockTransactions { get; set; }
    }
}
