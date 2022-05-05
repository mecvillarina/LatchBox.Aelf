using AElf.Contracts.MultiToken;
using AElf.CSharp.Core;
using AElf.Sdk.CSharp;
using Google.Protobuf.WellKnownTypes;
using System;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
    /// <summary>
    /// The C# implementation of the contract defined in multi_crowd_sale_contract.proto that is located in the "protobuf"
    /// folder.
    /// Notice that it inherits from the protobuf generated code. 
    /// </summary>
    public partial class MultiCrowdSaleContract : MultiCrowdSaleContractContainer.MultiCrowdSaleContractBase
    {
        public override Empty Initialize(Empty input)
        {
            Assert(State.Admin.Value == null, "Already initialized.");

            State.Admin.Value = Context.Sender;

            State.TokenContract.Value = Context.GetContractAddressByName(SmartContractConstants.TokenContractSystemName);
            State.ConsensusContract.Value = Context.GetContractAddressByName(SmartContractConstants.ConsensusContractSystemName);

            State.NativeToken.Value = State.TokenContract.GetNativeTokenInfo.Call(new Empty()).Symbol;
            State.SelfIncresingCrowdSaleId.Value = 1;

            return new Empty();
        }

        public override Empty Create(CreateCrowdSaleInput input)
        {
            AssertContractHasBeenInitialized();

            var currentBlockTime = Context.CurrentBlockTime;
            Assert(!string.IsNullOrEmpty(input.Name), "The parameter name is required.");
            AssertSymbolIsExists(input.TokenSymbol);
            AssertSymbolIssuerAndCrowdSaleInitiatorMustBeTheSame(input.TokenSymbol, Context.Sender);
            Assert(input.SoftCapNativeTokenAmount >= 0, "The parameter soft cap MUST be at least 0.");
            Assert(input.SoftCapNativeTokenAmount <= input.HardCapNativeTokenAmount, "The parameter soft cap MUST be less than hard cap.");
            Assert(input.SaleEndDate.ToDateTime() > currentBlockTime.ToDateTime(), "The parameter sale end date MUST be future date.");
            Assert(input.TokenAmountPerNativeToken > 0, "The parameter token amount per native token MUST be greater than 0.");
            Assert(input.NativeTokenLimitPerSale > 0, "The parameter native token limit per purchase MUST be greater than 0.");
            Assert(input.UnlockDate == null || input.UnlockDate.ToDateTime() > input.SaleEndDate.ToDateTime(), "The parameter unlock date MUST be future date and MUST be later than sale end date.");

            var nativeTokenInfo = GetNativeToken();
            var tokenInfo = State.TokenContract.GetTokenInfo.Call(new GetTokenInfoInput() { Symbol = input.TokenSymbol });

            // Add Token Transfer,

            var selfIncreasingId = State.SelfIncresingCrowdSaleId.Value;

            CrowdSale sale = new CrowdSale()
            {
                Id = State.SelfIncresingCrowdSaleId.Value,
                Initiator = Context.Sender,
                Name = input.Name,
                TokenSymbol = input.TokenSymbol,
                SoftCapNativeTokenAmount = input.SoftCapNativeTokenAmount,
                HardCapNativeTokenAmount = input.HardCapNativeTokenAmount,
                TokenAmountPerNativeToken = input.TokenAmountPerNativeToken,
                NativeTokenLimitPerSale = input.NativeTokenLimitPerSale,
                SaleEndDate = input.SaleEndDate,
                UnlockDate = input.UnlockDate,
                Pausable = input.Pausable,
                IsStarted = false,
                IsPaused = false
            };

            State.CrowdSales[selfIncreasingId] = sale;
            State.CrowdSaleRaiseAmounts[selfIncreasingId] = 0;

            var crowdSalesByInitiator = State.CrowdSalesByInitiator[Context.Sender];

            if (crowdSalesByInitiator == null)
            {
                crowdSalesByInitiator = new CrowdSaleList();
            }

            crowdSalesByInitiator.Ids.Add(selfIncreasingId);
            State.CrowdSalesByInitiator[Context.Sender] = crowdSalesByInitiator;

            State.SelfIncresingCrowdSaleId.Value = selfIncreasingId.Add(1);

            return new Empty();
        }

        public Empty Start(StartCrowdSaleInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.Id < State.SelfIncresingCrowdSaleId.Value, "Invalid Crowd Sale Id");

            var crowdSale = State.CrowdSales[input.Id];

            Assert(crowdSale.Initiator == Context.Sender || State.Admin.Value == Context.Sender, "No authorization.");
            Assert(!crowdSale.IsStarted, "Crowd Sale has already started");

            var crowdSaleId = input.Id;

            crowdSale.IsStarted = true;
            State.CrowdSales[crowdSale.Id] = crowdSale;

            var activeCrowdSaleIds = State.ActiveCrowdSales.Value;

            if (!activeCrowdSaleIds.Ids.Contains(crowdSaleId))
            {
                activeCrowdSaleIds.Ids.Add(crowdSale.Id);
                State.ActiveCrowdSales.Value = activeCrowdSaleIds;
            }


            return new Empty();
        }

        public Empty Cancel(CancelCrowdSaleInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.Id < State.SelfIncresingCrowdSaleId.Value, "Invalid Crowd Sale Id");

            var crowdSale = State.CrowdSales[input.Id];

            Assert(crowdSale.Initiator == Context.Sender || State.Admin.Value == Context.Sender, "No authorization.");
            Assert(crowdSale.IsStarted, "Crowd Sale is not yet started");
            Assert(crowdSale.SaleEndDate > Context.CurrentBlockTime, "Crowd Sale has already ended.");
            Assert(!crowdSale.IsCancelled, "Crowd Sale has already cancelled.");

            var crowdSaleId = input.Id;

            crowdSale.IsCancelled = true;
            State.CrowdSales[crowdSaleId] = crowdSale;

            var activeCrowdSaleIds = State.ActiveCrowdSales.Value;

            if (activeCrowdSaleIds.Ids.Contains(crowdSaleId))
            {
                activeCrowdSaleIds.Ids.Remove(crowdSaleId);
                State.ActiveCrowdSales.Value = activeCrowdSaleIds;
            }

            //Refunds
            return new Empty();
        }

        public Empty Pause(PauseCrowdSaleInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.Id < State.SelfIncresingCrowdSaleId.Value, "Invalid Crowd Sale Id");

            var crowdSale = State.CrowdSales[input.Id];

            Assert(crowdSale.Initiator == Context.Sender || State.Admin.Value == Context.Sender, "No authorization.");
            Assert(crowdSale.IsStarted, "Crowd Sale is not yet started");
            Assert(!crowdSale.IsCancelled, "Crowd Sale has been cancelled.");
            Assert(crowdSale.SaleEndDate > Context.CurrentBlockTime, "Crowd Sale has already ended.");
            Assert(!crowdSale.IsPaused, "Crowd Sale has already paused.");
            Assert(crowdSale.Pausable, "Crowd Sale is not pausable.");

            var crowdSaleId = input.Id;

            crowdSale.IsPaused = true;
            State.CrowdSales[crowdSaleId] = crowdSale;

            var activeCrowdSaleIds = State.ActiveCrowdSales.Value;

            if (activeCrowdSaleIds.Ids.Contains(crowdSaleId))
            {
                activeCrowdSaleIds.Ids.Remove(crowdSaleId);
                State.ActiveCrowdSales.Value = activeCrowdSaleIds;
            }

            return new Empty();
        }

        public Empty Resume(ResumeCrowdSaleInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.Id < State.SelfIncresingCrowdSaleId.Value, "Invalid Crowd Sale Id");

            var crowdSale = State.CrowdSales[input.Id];

            Assert(crowdSale.Initiator == Context.Sender || Context.Sender == State.Admin.Value, "No authorization.");
            Assert(crowdSale.IsStarted, "Crowd Sale is not yet started");
            Assert(!crowdSale.IsCancelled, "Crowd Sale has been cancelled.");
            Assert(crowdSale.SaleEndDate > Context.CurrentBlockTime, "Crowd Sale has already ended.");
            Assert(crowdSale.IsPaused, "Crowd Sale has not been paused.");

            var crowdSaleId = input.Id;
            crowdSale.IsPaused = false;
            State.CrowdSales[crowdSaleId] = crowdSale;

            var activeCrowdSaleIds = State.ActiveCrowdSales.Value;

            if (!activeCrowdSaleIds.Ids.Contains(crowdSaleId))
            {
                activeCrowdSaleIds.Ids.Add(crowdSale.Id);
                State.ActiveCrowdSales.Value = activeCrowdSaleIds;
            }

            return new Empty();
        }

        public Empty Buy(CrowdSaleBuyInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.Id < State.SelfIncresingCrowdSaleId.Value, "Invalid Crowd Sale Id");

            var crowdSale = State.CrowdSales[input.Id];
            Assert(crowdSale.IsStarted, "Crowd Sale is not yet started");
            Assert(!crowdSale.IsCancelled, "Crowd Sale has been cancelled.");
            Assert(!crowdSale.IsPaused, "Crowd Sale has been paused.");
            Assert(crowdSale.SaleEndDate > Context.CurrentBlockTime, "Crowd Sale has already ended.");

            var crowdSaleId = input.Id;

            var raiseAmount = State.CrowdSaleRaiseAmounts[crowdSaleId];
            var purchase = State.CrowdSalePurchases[crowdSaleId][Context.Sender];
            Assert(raiseAmount < crowdSale.HardCapNativeTokenAmount, "Crowd Sale Pool is already full.");

            long currentPurchaseAmount = 0;

            if (purchase != null)
            {
                currentPurchaseAmount = purchase.NativeTokenAmount;
            }

            Assert(currentPurchaseAmount <= crowdSale.NativeTokenLimitPerSale, "You've reached the buy limit per Address. ");
            Assert(currentPurchaseAmount + input.NativeTokenAmount <= crowdSale.NativeTokenLimitPerSale, "Your purchase will exceed the buy limit per address.");
            Assert(raiseAmount + input.NativeTokenAmount <= crowdSale.HardCapNativeTokenAmount, "Your purchase will exceed the crowd sale pool. ");

            if (purchase == null)
            {
                purchase = new CrowdSaleBuy();
            }

            //Transfer

            purchase.NativeTokenAmount += input.NativeTokenAmount;
            purchase.Investor = Context.Sender;
            purchase.DateLastPurchased = Context.CurrentBlockTime;

            State.CrowdSalePurchases[crowdSaleId][Context.Sender] = purchase;
            State.CrowdSaleRaiseAmounts[crowdSaleId] = State.CrowdSaleRaiseAmounts[crowdSaleId] + input.NativeTokenAmount;

            var crowdSalesByReceiver = State.CrowdSalesByBuyer[Context.Sender];

            if (crowdSalesByReceiver == null)
            {
                crowdSalesByReceiver = new CrowdSaleList();
            }

            if (!crowdSalesByReceiver.Ids.Contains(crowdSaleId))
            {
                crowdSalesByReceiver.Ids.Add(crowdSaleId);
                State.CrowdSalesByBuyer[Context.Sender] = crowdSalesByReceiver;
            }

            return new Empty();
        }

        //TBC
        public BoolValue Complete(CompleteCrowdSaleInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.Id < State.SelfIncresingCrowdSaleId.Value, "Invalid Crowd Sale Id");

            var crowdSale = State.CrowdSales[input.Id];
            var crowdSaleId = input.Id;

            Assert(crowdSale.Initiator == Context.Sender || Context.Sender == State.Admin.Value, "No authorization.");
            Assert(crowdSale.IsStarted, "Crowd Sale is not yet started");
            Assert(!crowdSale.IsCancelled, "Crowd Sale has been cancelled.");

            var raiseAmount = State.CrowdSaleRaiseAmounts[crowdSaleId];

            if (raiseAmount > crowdSale.SoftCapNativeTokenAmount)
            {
                return new BoolValue() { Value = true };
            }

            //refunds
            return new BoolValue() { Value = true };
        }
    }
}