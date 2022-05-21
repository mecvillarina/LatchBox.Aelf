using AElf.Contracts.MultiToken;
using AElf.CSharp.Core;
using AElf.Sdk.CSharp;
using Google.Protobuf.WellKnownTypes;
using System;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
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
            AssertSymbolExists(input.TokenSymbol);
            AssertSymbolIssuerAndCrowdSaleInitiatorMustBeTheSame(input.TokenSymbol, Context.Sender);
            Assert(input.SoftCapNativeTokenAmount > 0, "The parameter soft cap MUST be greater than 0.");
            Assert(input.SoftCapNativeTokenAmount <= input.HardCapNativeTokenAmount, "The parameter hard cap must be greater than or equal to soft cap.");
            Assert(input.SaleStartDate.ToDateTime() > currentBlockTime.ToDateTime(), "The parameter sale start date MUST be future date.");
            Assert(input.SaleEndDate.ToDateTime() > input.SaleStartDate.ToDateTime(), "The parameter sale end date MUST be later than sale start date.");
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

            var crowdSaleId = State.SelfIncresingCrowdSaleId.Value;
            State.SelfIncresingCrowdSaleId.Value = crowdSaleId.Add(1);

            CrowdSale sale = new CrowdSale()
            {
                Id = crowdSaleId,
                Initiator = Context.Sender,
                Name = input.Name,
                TokenSymbol = input.TokenSymbol,
                SoftCapNativeTokenAmount = input.SoftCapNativeTokenAmount,
                HardCapNativeTokenAmount = input.HardCapNativeTokenAmount,
                TokenAmountPerNativeToken = input.TokenAmountPerNativeToken,
                NativeTokenPurchaseLimitPerBuyerAddress = input.NativeTokenPurchaseLimitPerBuyerAddress,
                SaleStartDate = input.SaleStartDate,
                SaleEndDate = input.SaleEndDate,
                LockUntilDurationInMinutes = input.LockUntilDurationInMinutes,
                IsActive = true,
                IsCancelled = false
            };

            State.CrowdSales[crowdSaleId] = sale;
            State.CrowdSaleRaiseAmounts[crowdSaleId] = 0;
            State.CrowdSaleInvestors[crowdSaleId] = new CrowdSaleInvestorList();

            var activeCrowdSaleIds = State.ActiveCrowdSales.Value;

            if (!activeCrowdSaleIds.Ids.Contains(crowdSaleId))
            {
                activeCrowdSaleIds.Ids.Add(crowdSaleId);
                State.ActiveCrowdSales.Value = activeCrowdSaleIds;
            }

            var crowdSalesByInitiator = State.CrowdSalesByInitiator[Context.Sender];

            if (crowdSalesByInitiator == null)
            {
                crowdSalesByInitiator = new CrowdSaleIds();
            }

            crowdSalesByInitiator.Ids.Add(crowdSaleId);
            State.CrowdSalesByInitiator[Context.Sender] = crowdSalesByInitiator;


            return new Empty();
        }

        public override Empty Cancel(CancelInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Sale Id");

            var crowdSaleId = input.CrowdSaleId;
            var crowdSale = State.CrowdSales[crowdSaleId];

            Assert(crowdSale.Initiator == Context.Sender, "No authorization.");
            Assert(crowdSale.IsActive, "Sale is not active anymore.");
            Assert(!crowdSale.IsCancelled, "Sale has already cancelled.");

            State.TokenContract.Transfer.Send(new TransferInput()
            {
                To = crowdSale.Initiator,
                Amount = crowdSale.TokenAmountPerNativeToken * (crowdSale.HardCapNativeTokenAmount / GetChainAmount(10, State.NativeToken.Value.Decimals)),
                Symbol = crowdSale.TokenSymbol,
                Memo = "Refund"
            });

            //Add Event 

            var nativeTokenInfo = GetNativeToken();

            var investors = State.CrowdSaleInvestors[input.CrowdSaleId].Investors;

            foreach (var investor in investors)
            {
                var purchase = State.CrowdSaleInvestments[crowdSaleId][investor];
                if (purchase.DateRefunded == null)
                {
                    purchase.DateRefunded = Context.CurrentBlockTime;
                    State.TokenContract.Transfer.Send(new TransferInput()
                    {
                        To = purchase.Investor,
                        Amount = purchase.TokenAmount,
                        Symbol = nativeTokenInfo.Symbol,
                        Memo = "Refund"
                    });
                    State.CrowdSaleInvestments[crowdSaleId][investor] = purchase;
                    //Add Event
                }
            }

            crowdSale.IsCancelled = true;
            crowdSale.IsActive = false;
            State.CrowdSales[crowdSaleId] = crowdSale;

            RemoveFromActiveCrowdSales(crowdSaleId);

            return new Empty();
        }

        public override Empty Invest(InvestInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Sale Id");

            var crowdSaleId = input.CrowdSaleId;
            var crowdSale = State.CrowdSales[crowdSaleId];
            Assert(crowdSale.IsActive, "Sale is not active anymore.");
            Assert(!crowdSale.IsCancelled, "Sale has been cancelled.");
            Assert(Context.CurrentBlockTime.ToDateTime() > crowdSale.SaleStartDate.ToDateTime(), "Sale is not yet started.");
            Assert(Context.CurrentBlockTime.ToDateTime() > crowdSale.SaleEndDate.ToDateTime(), "Sale has already ended.");
            Assert(crowdSale.Initiator != Context.Sender, "Only the non-issuer (creator) of the token can buy on a crowd sale.");

            var raiseAmount = State.CrowdSaleRaiseAmounts[crowdSaleId];

            Assert(raiseAmount < crowdSale.HardCapNativeTokenAmount, "Sale Pool is already full.");

            var investment = State.CrowdSaleInvestments[crowdSaleId][Context.Sender];
            long currentPurchaseAmount = investment != null ? investment.TokenAmount : 0;

            Assert(currentPurchaseAmount <= crowdSale.NativeTokenPurchaseLimitPerBuyerAddress, "You've reached the buy limit. ");
            Assert(currentPurchaseAmount + input.TokenAmount <= crowdSale.NativeTokenPurchaseLimitPerBuyerAddress, "Your purchase will exceed the buy limit per address.");
            Assert(raiseAmount + input.TokenAmount <= crowdSale.HardCapNativeTokenAmount, "Your purchase will exceed the crowd sale pool. ");

            var nativeTokenInfo = GetNativeToken();

            var isNewInvestor = false;
            if (investment == null)
            {
                investment = new CrowdSaleInvestment();
                isNewInvestor = true;
            }

            State.TokenContract.TransferFrom.Send(new TransferFromInput()
            {
                From = Context.Sender,
                To = Context.Self,
                Symbol = nativeTokenInfo.Symbol,
                Amount = input.TokenAmount,
                Memo = $"Invest: {crowdSaleId}"
            });

            investment.TokenAmount += input.TokenAmount;
            investment.Investor = Context.Sender;
            investment.DateLastPurchased = Context.CurrentBlockTime;

            State.CrowdSaleInvestments[crowdSaleId][Context.Sender] = investment;
            State.CrowdSaleRaiseAmounts[crowdSaleId] = State.CrowdSaleRaiseAmounts[crowdSaleId] + input.TokenAmount;

            if (isNewInvestor)
            {
                var investorList = State.CrowdSaleInvestors[crowdSaleId];
                investorList.Investors.Add(Context.Sender);
                State.CrowdSaleInvestors[crowdSaleId] = investorList;
            }

            var crowdSalesByInvestor = State.CrowdSalesByInvestor[Context.Sender];

            if (crowdSalesByInvestor == null)
            {
                crowdSalesByInvestor = new CrowdSaleIds();
            }

            if (!crowdSalesByInvestor.Ids.Contains(crowdSaleId))
            {
                crowdSalesByInvestor.Ids.Add(crowdSaleId);
                State.CrowdSalesByInvestor[Context.Sender] = crowdSalesByInvestor;
            }

            return new Empty();
        }

        public override ResultOutput Complete(CompleteInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Sale Id");

            var crowdSaleId = input.CrowdSaleId;
            var crowdSale = State.CrowdSales[crowdSaleId];

            Assert(crowdSale.Initiator == Context.Sender || Context.Sender == State.Admin.Value, "No authorization.");
            Assert(crowdSale.IsActive, "Sale is not active anymore.");
            Assert(!crowdSale.IsCancelled, "Sale has been cancelled.");
            Assert(Context.CurrentBlockTime.ToDateTime() > crowdSale.SaleStartDate.ToDateTime(), "Sale is not yet started.");
            Assert(Context.CurrentBlockTime.ToDateTime() < crowdSale.SaleEndDate.ToDateTime(), "Sale is still ongoing not yet ended.");


            var raiseAmount = State.CrowdSaleRaiseAmounts[crowdSaleId];

            if (raiseAmount < crowdSale.SoftCapNativeTokenAmount)
            {
                State.TokenContract.Transfer.Send(new TransferInput()
                {
                    To = crowdSale.Initiator,
                    Amount = crowdSale.TokenAmountPerNativeToken * (crowdSale.HardCapNativeTokenAmount / GetChainAmount(10, State.NativeToken.Value.Decimals)),
                    Symbol = crowdSale.TokenSymbol,
                });

                crowdSale.IsSuccess = false;
                crowdSale.IsActive = false;

                State.CrowdSales[crowdSaleId] = crowdSale;
                RemoveFromActiveCrowdSales(crowdSaleId);

                return new ResultOutput { IsSuccess = false, Message = "Soft cap goal doesn't met. Your launchpad token was refunded." };
            }
            else
            {
                if (raiseAmount != crowdSale.HardCapNativeTokenAmount)
                {
                    State.TokenContract.Transfer.Send(new TransferInput()
                    {
                        To = crowdSale.Initiator,
                        Amount = crowdSale.TokenAmountPerNativeToken * ((crowdSale.HardCapNativeTokenAmount - raiseAmount) / GetChainAmount(10, State.NativeToken.Value.Decimals)),
                        Symbol = crowdSale.TokenSymbol,
                    });
                }

                State.TokenContract.Transfer.Send(new TransferInput()
                {
                    To = Context.Sender,
                    Amount = crowdSale.TokenAmountPerNativeToken * (raiseAmount / GetChainAmount(10, State.NativeToken.Value.Decimals)),
                    Symbol = crowdSale.TokenSymbol,
                });

                State.TokenContract.Transfer.Send(new TransferInput()
                {
                    To = crowdSale.Initiator,
                    Amount = raiseAmount,
                    Symbol = State.NativeToken.Value.Symbol
                });

                crowdSale.IsSuccess = true;
                crowdSale.IsActive = false;
                State.CrowdSales[crowdSaleId] = crowdSale;
                RemoveFromActiveCrowdSales(crowdSaleId);

                return new ResultOutput { IsSuccess = true };
            }

        }

        public override Empty UpdateLockInfo(UpdateLockInfoInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Sale Id");

            var crowdSaleId = input.CrowdSaleId;
            var crowdSale = State.CrowdSales[crowdSaleId];

            Assert(crowdSale.Initiator == Context.Sender || Context.Sender == State.Admin.Value, "No authorization.");
            Assert(!crowdSale.IsActive, "Sale is still active.");
            Assert(crowdSale.IsSuccess, "Sale doesn't met the soft cap goal.");
            Assert(crowdSale.LockId == 0, "Sale has already associated lock.");
            Assert(input.LockId > 0, "Lock doesn't exists.");

            crowdSale.LockId = input.LockId;
            State.CrowdSales[crowdSaleId] = crowdSale;

            return new Empty();
        }
    }
}