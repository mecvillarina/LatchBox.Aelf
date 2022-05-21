using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp.State;
using AElf.Types;

namespace LatchBox.Contracts.VestingTokenVaultContract
{
    public class VestingTokenVaultContractState : ContractState
    {
        internal TokenContractContainer.TokenContractReferenceState TokenContract { get; set; }

        public SingletonState<Address> Admin { get; set; }

        public SingletonState<long> SelfIncresingVestingId { get; set; }
        public SingletonState<long> SelfIncresingPeriodId { get; set; }

        public MappedState<long, Vesting> Vestings { get; set; }
        public MappedState<long, long, VestingPeriod> VestingPeriods { get; set; }
        public MappedState<long, VestingPeriodIdList> VestingPeriodList { get; set; }

        public MappedState<long, Address, VestingReceiver> VestingReceivers { get; set; }
        public MappedState<long, VestingReceiverAddressList> VestingReceiverList { get; set; } //long period

        public MappedState<Address, VestingIdList> VestingListByInitiator { get; set; }
        public MappedState<Address, VestingReceiverPeriodList> VestingListForReceivers { get; set; }
        public MappedState<Address, RefundList> Refunds { get; set; }

    }
}