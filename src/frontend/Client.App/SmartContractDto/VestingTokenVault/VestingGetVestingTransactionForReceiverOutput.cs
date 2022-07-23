namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingGetVestingTransactionForReceiverOutput
    {
        public VestingOutput Vesting { get; set; }
        public VestingPeriodOutput Period { get; set; }
        public VestingReceiverOutput Receiver { get; set; }
    }
}
