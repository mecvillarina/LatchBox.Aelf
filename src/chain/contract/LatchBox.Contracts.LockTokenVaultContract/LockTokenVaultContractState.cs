using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp.State;
using AElf.Types;

namespace LatchBox.Contracts.LockTokenVaultContract
{
    /// <summary>
    /// The state class of the contract, it inherits from the AElf.Sdk.CSharp.State.ContractState type. 
    /// </summary>
    public class LockTokenVaultContractState : ContractState
    {
        internal TokenContractContainer.TokenContractReferenceState TokenContract { get; set; }

        public SingletonState<Address> Admin { get; set; }
        public SingletonState<long> SelfIncresingLockId { get; set; }

        public MappedState<long, Lock> Locks { get; set; }
        public MappedState<long, Address, LockReceiver> LockReceivers { get; set; }
        public MappedState<long, LockReceiverAddressList> LockReceiverList { get; set; }

        public MappedState<Address, LockIdList> LockListByInitiator { get; set; }
        public MappedState<Address, LockIdList> LockListForReceiver { get; set; }
        public MappedState<string, LockIdList> LockAssetIdList { get; set; }
        public MappedState<string, LockAssetCounter> AssetCounter { get; set; }
        public SingletonState<LockAssetCounterTokenSymbolList> AssetCounterList { get; set; }
        public MappedState<Address, RefundList> Refunds { get; set; }
    }
}