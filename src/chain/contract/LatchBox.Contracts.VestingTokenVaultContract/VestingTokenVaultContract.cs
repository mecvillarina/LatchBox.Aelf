using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using System.Linq;

namespace LatchBox.Contracts.VestingTokenVaultContract
{
    public partial class VestingTokenVaultContract : VestingTokenVaultContractContainer.VestingTokenVaultContractBase
    {
        public override Empty Initialize(Empty input)
        {
            Assert(State.Admin.Value == null, "Already initialized");

            State.Admin.Value = Context.Sender;

            State.TokenContract.Value = Context.GetContractAddressByName(SmartContractConstants.TokenContractSystemName);

            State.SelfIncresingVestingId.Value = 1;
            State.SelfIncresingPeriodId.Value = 1;

            return new Empty();
        }

        public override Empty AddVesting(AddVestingInput input)
        {
            AssertContractHasBeenInitialized();

            var currentBlockTime = Context.CurrentBlockTime;

            AssertSymbolExists(input.TokenSymbol);
            Assert(input.TotalAmount > 0, "The parameter total amount MUST be greater than 0.");
            AssertValidVestingPeriodInputData(input.Periods, input.TotalAmount);

            State.TokenContract.TransferFrom.Send(new TransferFromInput()
            {
                From = Context.Sender,
                To = Context.Self,
                Symbol = input.TokenSymbol,
                Amount = input.TotalAmount,
                Memo = $"{nameof(VestingTokenVaultContract)}: Add Vesting",
            });

            var vestingId = State.SelfIncresingVestingId.Value;
            State.SelfIncresingVestingId.Value = vestingId + 1;

            var vesting = new Vesting()
            {
                VestingId = vestingId,
                TokenSymbol = input.TokenSymbol,
                CreationTime = currentBlockTime,
                TotalAmount = input.TotalAmount, 
                Initiator = Context.Sender,
                IsRevocable = input.IsRevocable,
                IsActive = true,
                IsRevoked = false,
            };

            State.Vestings[vestingId] = vesting;
            UpdatVestingInitiatorIds(Context.Sender, vestingId);

            foreach (var periodData in input.Periods)
            {
                var periodId = State.SelfIncresingPeriodId.Value;
                State.SelfIncresingPeriodId.Value = periodId + 1;

                var vestingPeriod = new VestingPeriod()
                {
                    PeriodId = periodId,
                    Name = periodData.Name,
                    TotalAmount = periodData.TotalAmount,
                    UnlockTime = periodData.UnlockTime
                };

                AddVestingPeriod(vestingId, vestingPeriod);

                foreach (var receiverData in periodData.Receivers)
                {
                    var receiver = new VestingReceiver()
                    {
                        Address = receiverData.Address,
                        Amount = receiverData.Amount,
                        DateClaimed = null,
                        DateRevoked = null,
                        Name = receiverData.Name
                    };

                    AddVestingReceivers(periodId, receiver);
                    UpdateVestingReceiverPeriodList(receiver.Address, vestingId, periodId);
                }
            }

            Context.Fire(new OnCreatedVestingEvent
            {
                VestingId = vestingId
            });

            return new Empty();
        }

        public override Empty ClaimVesting(ClaimVestingInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.VestingId < State.SelfIncresingVestingId.Value && input.PeriodId < State.SelfIncresingPeriodId.Value, "Vesting doesn't exists.");

            var vestingObj = State.Vestings[input.VestingId];
            var periodObj = State.VestingPeriods[input.VestingId][input.PeriodId];
            Assert(vestingObj != null && periodObj != null, "Vesting doesn't exists.");

            var receiverObj = State.VestingReceivers[periodObj.PeriodId][Context.Sender];
            Assert(receiverObj != null, "No authorization.");

            if (vestingObj.IsRevoked || (receiverObj.DateRevoked != null)) throw new AssertionException("Vesting has been already revoked by the initiator.");

            Assert(vestingObj.IsActive, "Vesting is not active anymore.");
            Assert(receiverObj.DateClaimed == null, "Vesting has been claimed.");
            Assert(Context.CurrentBlockTime.ToDateTime() > periodObj.UnlockTime.ToDateTime(), "Vesting selected period is not yet ready to be claimed.");

            State.TokenContract.Transfer.Send(new TransferInput()
            {
                To = receiverObj.Address,
                Amount = receiverObj.Amount,
                Memo = $"{nameof(VestingTokenVaultContract)}: Claim Vesting",
                Symbol = vestingObj.TokenSymbol
            });

            receiverObj.DateClaimed = Context.CurrentBlockTime;

            State.VestingReceivers[periodObj.PeriodId][Context.Sender] = receiverObj;

            var periodIds = State.VestingPeriodList[vestingObj.VestingId].PeriodIds.ToList();
            bool isAllClaimed = true;
            foreach (var periodId in periodIds)
            {
                var receiverAddressList = State.VestingReceiverList[periodId].Receivers.ToList();
                foreach (var receiverAddress in receiverAddressList)
                {
                    var receiver = State.VestingReceivers[periodId][receiverAddress];
                    if (receiver.DateClaimed == null)
                    {
                        isAllClaimed = false;
                        break;
                    }
                }
            }

            if (isAllClaimed)
            {
                vestingObj.IsActive = false;
                State.Vestings[vestingObj.VestingId] = vestingObj;
            }

            Context.Fire(new OnClaimedVestingEvent
            {
                VestingId = vestingObj.VestingId,
                PeriodId = periodObj.PeriodId,
                Receiver = receiverObj.Address,
                ClaimedAmount = receiverObj.Amount
            });

            return new Empty();
        }

        public override Empty RevokeVesting(RevokeVestingInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.VestingId < State.SelfIncresingVestingId.Value, "Vesting doesn't exists.");

            var vestingObj = State.Vestings[input.VestingId];

            Assert(Context.Sender == vestingObj.Initiator, "No authorization.");
            Assert(!vestingObj.IsRevoked, "Vesting has been already revoked by the initiator.");
            Assert(vestingObj.IsActive, "Vesting is not active anymore.");
            Assert(vestingObj.IsRevocable, "Vesting is irrevocable.");

            vestingObj.IsRevoked = true;
            vestingObj.IsActive = false;

            State.Vestings[vestingObj.VestingId] = vestingObj;

            long totalRefundAmount = 0;
            var periodIds = State.VestingPeriodList[vestingObj.VestingId].PeriodIds.ToList();
            foreach (var periodId in periodIds)
            {
                var receiverAddressList = State.VestingReceiverList[periodId].Receivers.ToList();
                foreach (var receiverAddress in receiverAddressList)
                {
                    var receiver = State.VestingReceivers[periodId][receiverAddress];
                    if (receiver.DateClaimed == null && receiver.DateRevoked == null)
                    {
                        receiver.DateRevoked = Context.CurrentBlockTime;
                        totalRefundAmount += receiver.Amount;
                        UpdateRefunds(vestingObj.Initiator, vestingObj.TokenSymbol, receiver.Amount);
                        State.VestingReceivers[periodId][receiverAddress] = receiver;
                    }
                }
            }

            Context.Fire(new OnRevokedVestingEvent
            {
                VestingId = vestingObj.VestingId,
                UnlockedAmount = totalRefundAmount,
            });
            return new Empty();
        }

        public override Empty ClaimRefund(ClaimRefundInput input)
        {
            AssertContractHasBeenInitialized();

            AssertSymbolExists(input.TokenSymbol);

            var refundsObj = State.Refunds[Context.Sender];
            Assert(refundsObj != null, "No refund found.");

            var refund = refundsObj.Refunds.FirstOrDefault(x => x.TokenSymbol == input.TokenSymbol);
            Assert(refund != null, "No refund found.");

            State.TokenContract.Transfer.Send(new TransferInput()
            {
                To = Context.Sender,
                Amount = refund.Amount,
                Memo = $"{nameof(VestingTokenVaultContract)}: Claim Refund",
                Symbol = refund.TokenSymbol
            });

            refundsObj.Refunds.Remove(refund);
            State.Refunds[Context.Sender] = refundsObj;

            Context.Fire(new OnClaimedRefundEvent
            {
                Recipient = Context.Sender,
                TokenSymbol = refund.TokenSymbol,
                RefundedAmount = refund.Amount
            });

            return new Empty();
        }

        private void UpdateRefunds(Address initiator, string tokenSymbol, long amount)
        {
            var refundsObj = State.Refunds[initiator];

            if (refundsObj == null)
            {
                refundsObj = new RefundList();
            }

            var refund = refundsObj.Refunds.FirstOrDefault(x => x.TokenSymbol == tokenSymbol);

            if (refund != null)
            {
                refund.Amount += amount;
            }
            else
            {
                refund = new Refund()
                {
                    TokenSymbol = tokenSymbol,
                    Amount = amount
                };
                refundsObj.Refunds.Add(refund);
            }

            State.Refunds[initiator] = refundsObj;
        }

        private void AddVestingReceivers(long periodId, VestingReceiver receiverObj)
        {
            State.VestingReceivers[periodId][receiverObj.Address] = receiverObj;

            if (State.VestingReceiverList[periodId] == null)
            {
                State.VestingReceiverList[periodId] = new VestingReceiverAddressList();
            }

            var currentList = State.VestingReceiverList[periodId];
            currentList.Receivers.Add(receiverObj.Address);
            State.VestingReceiverList[periodId] = currentList;
        }

        private void AddVestingPeriod(long vestingId, VestingPeriod periodObj)
        {
            State.VestingPeriods[vestingId][periodObj.PeriodId] = periodObj;

            if (State.VestingPeriodList[vestingId] == null)
            {
                State.VestingPeriodList[vestingId] = new VestingPeriodIdList();
            }

            var currentList = State.VestingPeriodList[vestingId];
            currentList.PeriodIds.Add(periodObj.PeriodId);
            State.VestingPeriodList[vestingId] = currentList;
        }

        private void UpdatVestingInitiatorIds(Address initiator, long vestingId)
        {
            if (State.VestingListByInitiator[initiator] == null)
            {
                State.VestingListByInitiator[initiator] = new VestingIdList();
            }

            var currentList = State.VestingListByInitiator[initiator];
            currentList.Ids.Add(vestingId);
            State.VestingListByInitiator[initiator] = currentList;
        }

        private void UpdateVestingReceiverPeriodList(Address receiver, long vestingId, long periodId)
        {
            if (State.VestingListForReceivers[receiver] == null)
            {
                State.VestingListForReceivers[receiver] = new VestingReceiverPeriodList();
            }

            var currentList = State.VestingListForReceivers[receiver];
            currentList.List.Add(new VestingReceiverPeriod()
            {
                VestingId = vestingId,
                PeriodId = periodId
            });
            State.VestingListForReceivers[receiver] = currentList;
        }
    }
}