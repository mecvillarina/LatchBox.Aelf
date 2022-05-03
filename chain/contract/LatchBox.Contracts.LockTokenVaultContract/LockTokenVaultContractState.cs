using AElf.Contracts.Consensus.AEDPoS;
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
        internal AEDPoSContractContainer.AEDPoSContractReferenceState ConsensusContract { get; set; }

        public SingletonState<Address> Admin { get; set; }
        public SingletonState<TokenVaultSettings> TokenVaultSettings { get; set; }
        public SingletonState<LatchBoxPayment> Payment { get; set; }

        public UInt64State LockIndex { get; set; }

        public MappedState<ulong, LatchBoxLock> Locks { get; set; }
        public MappedState<Address, LockIndexList> LockInitiatorIndexes { get; set; }
        public MappedState<Address, LockIndexList> LockReceiverIndexes { get; set; }
        public MappedState<string, LockIndexList> LockAssetIndexes { get; set; }
        public MappedState<string, LockAssetCounter> AssetCounter { get; set; }
    }
}