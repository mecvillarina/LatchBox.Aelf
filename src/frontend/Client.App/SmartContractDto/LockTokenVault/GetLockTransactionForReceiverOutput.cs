namespace Client.App.SmartContractDto.LockTokenVault
{
    public class GetLockTransactionForReceiverOutput
    {
        public LockOutput Lock { get; set; }
        public LockReceiverOutput Receiver { get; set; }
    }
}
