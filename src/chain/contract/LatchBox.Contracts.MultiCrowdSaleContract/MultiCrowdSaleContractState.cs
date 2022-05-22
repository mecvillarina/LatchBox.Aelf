using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp.State;
using AElf.Types;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
    /// <summary>
    /// The state class of the contract, it inherits from the AElf.Sdk.CSharp.State.ContractState type. 
    /// </summary>
    public class MultiCrowdSaleContractState : ContractState
    {
        internal TokenContractContainer.TokenContractReferenceState TokenContract { get; set; }

        public SingletonState<Address> Admin { get; set; }
        public SingletonState<NativeToken> NativeToken { get; set; }
        public SingletonState<long> SelfIncresingCrowdSaleId { get; set; }

        public MappedState<long, CrowdSale> CrowdSales { get; set; }
        public MappedState<long, long> CrowdSaleRaiseAmounts { get; set; }
        public SingletonState<CrowdSaleIds> ActiveCrowdSales { get; set; }
        public MappedState<long, Address, CrowdSaleInvestment> CrowdSaleInvestments { get; set; }
        public MappedState<long, CrowdSaleInvestorList> CrowdSaleInvestors { get; set; }

        public MappedState<Address, CrowdSaleIds> CrowdSalesByInitiator { get; set; }
        public MappedState<Address, CrowdSaleIds> CrowdSalesByInvestor { get; set; }
    }
}