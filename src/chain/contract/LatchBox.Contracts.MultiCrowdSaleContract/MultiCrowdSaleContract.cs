using AElf.Contracts.MultiToken;
using AElf.CSharp.Core;
using AElf.Sdk.CSharp;
using AElf.Types;
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

            var nativeToken = GetNativeToken();

            State.NativeToken.Value = new NativeToken()
            {
                Symbol = nativeToken.Symbol,
                TokenName = nativeToken.TokenName,
                Decimals = nativeToken.Decimals
            };

            State.SelfIncresingCrowdSaleId.Value = 1;
            State.ActiveCrowdSales.Value = new CrowdSaleIds();
            return new Empty();
        }

        public override Empty Create(CreateInput input)
        {
            AssertContractHasBeenInitialized();

            var currentBlockTime = Context.CurrentBlockTime;
            Assert(!string.IsNullOrEmpty(input.Name), "The parameter name is required.");
            AssertSymbolIsExists(input.TokenSymbol);
            AssertSymbolIssuerAndCrowdSaleInitiatorMustBeTheSame(input.TokenSymbol, Context.Sender);
            Assert(input.SoftCapNativeTokenAmount > 0, "The parameter soft cap MUST be greater than 0.");
            Assert(input.SoftCapNativeTokenAmount <= input.HardCapNativeTokenAmount, "The parameter hard cap must be greater than or equal to soft cap.");
            Assert(input.SaleEndDate.ToDateTime() > currentBlockTime.ToDateTime(), "The parameter sale end date MUST be future date.");
            Assert(input.TokenAmountPerNativeToken > 0, "The parameter token amount per native token MUST be greater than 0.");
            Assert(input.NativeTokenPurchaseLimitPerBuyerAddress > 0, "The parameter native token limit per buyer address MUST be greater than 0.");
            Assert(input.NativeTokenPurchaseLimitPerBuyerAddress <= input.HardCapNativeTokenAmount, "The parameter token limit per buyer address MUST be less than the hard cap.");
            Assert(input.LockUntilDurationInMinutes >= 60, "The parameter lock until duration MUST be at least 1 hour.");

            var totalAmount = input.TokenAmountPerNativeToken * (input.HardCapNativeTokenAmount / GetChainAmount(10, State.NativeToken.Value.Decimals));

            State.TokenContract.TransferFrom.Send(new TransferFromInput()
            {
                From = Context.Sender,
                To = Context.Self, 
                Symbol = input.TokenSymbol,
                Amount = totalAmount,
                Memo = input.Name,
            });

            var selfIncreasingId = State.SelfIncresingCrowdSaleId.Value;

            CrowdSale sale = new CrowdSale()
            {
                Id = selfIncreasingId,
                Initiator = Context.Sender,
                Name = input.Name,
                TokenSymbol = input.TokenSymbol,
                SoftCapNativeTokenAmount = input.SoftCapNativeTokenAmount,
                HardCapNativeTokenAmount = input.HardCapNativeTokenAmount,
                TokenAmountPerNativeToken = input.TokenAmountPerNativeToken,
                NativeTokenPurchaseLimitPerBuyerAddress = input.NativeTokenPurchaseLimitPerBuyerAddress,
                SaleEndDate = input.SaleEndDate,
                LockUntilDurationInMinutes = input.LockUntilDurationInMinutes,
                IsActive = true,
                IsCancelled = false
            };

            State.CrowdSales[selfIncreasingId] = sale;
            State.CrowdSaleRaiseAmounts[selfIncreasingId] = 0;

            var activeCrowdSaleIds = State.ActiveCrowdSales.Value;

            if (!activeCrowdSaleIds.Ids.Contains(selfIncreasingId))
            {
                activeCrowdSaleIds.Ids.Add(selfIncreasingId);
                State.ActiveCrowdSales.Value = activeCrowdSaleIds;
            }

            var crowdSalesByInitiator = State.CrowdSalesByInitiator[Context.Sender];

            if (crowdSalesByInitiator == null)
            {
                crowdSalesByInitiator = new CrowdSaleIds();
            }

            crowdSalesByInitiator.Ids.Add(selfIncreasingId);
            State.CrowdSalesByInitiator[Context.Sender] = crowdSalesByInitiator;

            State.SelfIncresingCrowdSaleId.Value = selfIncreasingId.Add(1);

            return new Empty();
        }

        public override Empty Cancel(CancelInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Crowd Sale Id");
            
            var crowdSaleId = input.CrowdSaleId;
            var crowdSale = State.CrowdSales[crowdSaleId];

            Assert(crowdSale.Initiator == Context.Sender || State.Admin.Value == Context.Sender, "No authorization.");
            Assert(crowdSale.IsActive, "Crowd Sale is not active anymore.");
            Assert(crowdSale.SaleEndDate > Context.CurrentBlockTime, "Crowd Sale has already ended.");
            Assert(!crowdSale.IsCancelled, "Crowd Sale has already cancelled.");

            State.TokenContract.Transfer.Send(new TransferInput()
            {
                To = crowdSale.Initiator,
                Amount = crowdSale.TokenAmountPerNativeToken * (crowdSale.HardCapNativeTokenAmount / GetChainAmount(10, State.NativeToken.Value.Decimals)),
                Symbol = crowdSale.TokenSymbol,
                Memo = "Refund"
            });

            //Refund for Buyer
            crowdSale.IsCancelled = true;
            crowdSale.IsActive = false;
            State.CrowdSales[crowdSaleId] = crowdSale;

            RemoveFromActiveCrowdSales(crowdSaleId);

            return new Empty();
        }

        public Empty Buy(BuyInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Crowd Sale Id");

            var crowdSaleId = input.CrowdSaleId;
            var crowdSale = State.CrowdSales[crowdSaleId];
            Assert(crowdSale.IsActive, "Crowd Sale is not active anymore.");
            Assert(!crowdSale.IsCancelled, "Crowd Sale has been cancelled.");
            Assert(crowdSale.SaleEndDate > Context.CurrentBlockTime, "Crowd Sale has already ended.");
            Assert(crowdSale.Initiator != Context.Sender, "Only the non-issuer (creator) of the token can buy on a crowd sale.");

            var raiseAmount = State.CrowdSaleRaiseAmounts[crowdSaleId];
            var purchase = State.CrowdSalePurchases[crowdSaleId][Context.Sender];
            Assert(raiseAmount < crowdSale.HardCapNativeTokenAmount, "Crowd Sale Pool is already full.");

            long currentPurchaseAmount = purchase != null ? purchase.TokenAmount : 0;

            Assert(currentPurchaseAmount <= crowdSale.NativeTokenPurchaseLimitPerBuyerAddress, "You've reached the buy limit. ");
            Assert(currentPurchaseAmount + input.TokenAmount <= crowdSale.NativeTokenPurchaseLimitPerBuyerAddress, "Your purchase will exceed the buy limit per address.");
            Assert(raiseAmount + input.TokenAmount <= crowdSale.HardCapNativeTokenAmount, "Your purchase will exceed the crowd sale pool. ");

            if (purchase == null)
            {
                purchase = new CrowdSalePurchase();
            }

            State.TokenContract.TransferFrom.Send(new TransferFromInput()
            {
                From = Context.Sender,
                To = Context.Self,
                Symbol = crowdSale.TokenSymbol,
                Amount = input.TokenAmount,
                Memo = $"{crowdSaleId}"
            });

            purchase.TokenAmount += input.TokenAmount;
            purchase.Investor = Context.Sender;
            purchase.DateLastPurchased = Context.CurrentBlockTime;
            
            State.CrowdSalePurchases[crowdSaleId][Context.Sender] = purchase;
            State.CrowdSaleRaiseAmounts[crowdSaleId] = State.CrowdSaleRaiseAmounts[crowdSaleId] + input.TokenAmount;

            var crowdSalesByReceiver = State.CrowdSalesByBuyer[Context.Sender];

            if (crowdSalesByReceiver == null)
            {
                crowdSalesByReceiver = new CrowdSaleIds();
            }

            if (!crowdSalesByReceiver.Ids.Contains(crowdSaleId))
            {
                crowdSalesByReceiver.Ids.Add(crowdSaleId);
                State.CrowdSalesByBuyer[Context.Sender] = crowdSalesByReceiver;
            }

            return new Empty();
        }

        //TBC
        public override Empty Complete(CompleteInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Crowd Sale Id");

            var crowdSaleId = input.CrowdSaleId;
            var crowdSale = State.CrowdSales[crowdSaleId];

            Assert(crowdSale.Initiator == Context.Sender || Context.Sender == State.Admin.Value, "No authorization.");
            Assert(crowdSale.IsActive, "Crowd Sale is not active anymore.");
            Assert(!crowdSale.IsCancelled, "Crowd Sale has been cancelled.");
            var raiseAmount = State.CrowdSaleRaiseAmounts[crowdSaleId];

            Assert(raiseAmount >= crowdSale.SoftCapNativeTokenAmount, "Crowd Sale doesn't met at least the soft cap goal.");

            RemoveFromActiveCrowdSales(crowdSaleId);

            return new Empty();
        }

    }
}