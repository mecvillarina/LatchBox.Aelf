using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using System.Linq;

namespace LatchBox.Contracts.LockTokenVaultContract
{
    public partial class LockTokenVaultContract : LockTokenVaultContractContainer.LockTokenVaultContractBase
    {
        public override Empty Initialize(InitializeInput input)
        {
            Assert(State.Admin.Value == null, "Already initialized");

            State.Admin.Value = Context.Sender;

            State.TokenContract.Value = Context.GetContractAddressByName(SmartContractConstants.TokenContractSystemName);
            State.ConsensusContract.Value = Context.GetContractAddressByName(SmartContractConstants.ConsensusContractSystemName);

            State.SelfIncresingLockId.Value = 1;
            State.AssetCounterList.Value = new LockAssetCounterTokenSymbolList();

            return new Empty();
        }

        public override Empty AddLock(AddLockInput input)
        {
            AssertContractHasBeenInitialized();

            var currentBlockTime = Context.CurrentBlockTime;

            AssertSymbolExists(input.TokenSymbol);
            Assert(input.TotalAmount > 0, "The parameter total amount MUST be greater than 0.");
            Assert(input.UnlockTime.ToDateTime() >= currentBlockTime.ToDateTime(), "The parameter unlock time MUST be future date.");
            AssertValidLockReceiverInputData(input.Receivers, input.TotalAmount);

            State.TokenContract.TransferFrom.Send(new TransferFromInput()
            {
                From = Context.Sender,
                To = Context.Self,
                Symbol = input.TokenSymbol,
                Amount = input.TotalAmount,
                Memo = $"{nameof(LockTokenVaultContract)}: Add Lock",
            });

            var lockId = State.SelfIncresingLockId.Value;
            State.SelfIncresingLockId.Value = lockId + 1;

            var initiator = Context.Sender;

            var lockObj = new Lock
            {
                LockId = lockId,
                TokenSymbol = input.TokenSymbol,
                Initiator = initiator,
                TotalAmount = input.TotalAmount,
                CreationTime = currentBlockTime,
                StartTime = currentBlockTime,
                UnlockTime = input.UnlockTime,
                IsActive = true,
                IsRevocable = input.IsRevocable,
                IsRevoked = false
            };

            State.Locks[lockId] = lockObj;
            UpdateLockInitiatorIds(initiator, lockId);

            var receiverAddressList = new LockReceiverAddressList();

            foreach (var receiverInput in input.Receivers)
            {
                LockReceiver lockReceiver = new LockReceiver
                {
                    Receiver = receiverInput.Receiver,
                    Amount = receiverInput.Amount,
                    DateClaimed = null,
                    DateRevoked = null,
                    IsActive = true
                };

                State.LockReceivers[lockId][receiverInput.Receiver] = lockReceiver;
                UpdateLockReceiversIds(lockReceiver.Receiver, lockId);
                receiverAddressList.Receivers.Add(lockReceiver.Receiver);
            }

            State.LockReceiverList[lockId] = receiverAddressList;

            UpdateLockAssetIds(input.TokenSymbol, lockId);
            IncrementAssetLockedCounter(input.TokenSymbol, input.TotalAmount);
            Context.Fire(new OnCreatedLockEvent
            {
                LockId = lockId
            });

            return new Empty();
        }

        public override Empty ClaimLock(ClaimLockInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.LockId < State.SelfIncresingLockId.Value, "Lock doesn't exists.");

            var lockObj = State.Locks[input.LockId];

            var receiverObj = State.LockReceivers[lockObj.LockId][Context.Sender];
            Assert(receiverObj != null, "No authorization.");

            if (lockObj.IsRevoked || (!receiverObj.IsActive && receiverObj.DateRevoked != null)) throw new AssertionException("Lock has been already revoked by the initiator.");

            Assert(lockObj.IsActive, "Lock is not active anymore");

            if (!receiverObj.IsActive && receiverObj.DateClaimed != null) throw new AssertionException("Lock has been already claimed.");

            //Assert(Context.CurrentBlockTime > lockObj.UnlockTime, "Lock is not yet ready to be claimwd");

            State.TokenContract.Transfer.Send(new TransferInput()
            {
                To = receiverObj.Receiver,
                Amount = receiverObj.Amount,
                Memo = $"{nameof(LockTokenVaultContract)}: Claim Lock",
                Symbol = lockObj.TokenSymbol
            });

            receiverObj.DateClaimed = Context.CurrentBlockTime;
            receiverObj.IsActive = false;

            State.LockReceivers[lockObj.LockId][Context.Sender] = receiverObj;
            IncrementAssetUnlockedCounter(lockObj.TokenSymbol, receiverObj.Amount);

            var receiverAddressList = State.LockReceiverList[lockObj.LockId].Receivers.ToList();
            bool isAllClaimed = true;
            foreach (var receiverAddress in receiverAddressList)
            {
                var receiver = State.LockReceivers[lockObj.LockId][receiverAddress];
                if (receiver.IsActive && receiver.DateClaimed == null)
                {
                    isAllClaimed = false;
                    break;
                }
            }

            if (isAllClaimed)
            {
                lockObj.IsActive = false;
                State.Locks[lockObj.LockId] = lockObj;
            }

            Context.Fire(new OnClaimedLockEvent
            {
                LockId = lockObj.LockId,
                Receiver = receiverObj.Receiver,
                ClaimedAmount = receiverObj.Amount
            });

            return new Empty();
        }

        public override Empty RevokeLock(RevokeLockInput input)
        {
            AssertContractHasBeenInitialized();

            Assert(input.LockId < State.SelfIncresingLockId.Value, "Lock doesn't exists.");

            var lockObj = State.Locks[input.LockId];

            Assert(lockObj.Initiator == Context.Sender, "No authorization.");
            Assert(!lockObj.IsRevoked, "Lock has been already revoked by the initiator.");
            Assert(lockObj.IsActive, "Lock is not active anymore.");
            Assert(lockObj.IsRevocable, "Lock is irrevocable.");

            lockObj.IsRevoked = true;
            lockObj.IsActive = false;

            State.Locks[lockObj.LockId] = lockObj;

            long totalRefundAmount = 0;

            var receiverAddressList = State.LockReceiverList[lockObj.LockId].Receivers.ToList();
            foreach (var receiverAddress in receiverAddressList)
            {
                var lockReceiver = State.LockReceivers[lockObj.LockId][receiverAddress];
                if (lockReceiver.DateClaimed == null && lockReceiver.DateRevoked == null)
                {
                    lockReceiver.DateRevoked = Context.CurrentBlockTime;
                    lockReceiver.IsActive = false;
                    totalRefundAmount += lockReceiver.Amount;
                    UpdateRefunds(lockObj.Initiator, lockObj.TokenSymbol, lockReceiver.Amount);
                    State.LockReceivers[lockObj.LockId][receiverAddress] = lockReceiver;
                }
            }

            Context.Fire(new OnRevokeLockEvent
            {
                LockId = lockObj.LockId,
                UnlockedAmount = totalRefundAmount
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
                Memo = $"{nameof(LockTokenVaultContract)}: Claim Refund",
                Symbol = refund.TokenSymbol
            });

            refundsObj.Refunds.Remove(refund);
            State.Refunds[Context.Sender] = refundsObj;

            IncrementAssetUnlockedCounter(input.TokenSymbol, refund.Amount);

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

        private void UpdateLockInitiatorIds(Address initiator, long lockId)
        {
            if (State.LockListByInitiator[initiator] == null)
            {
                State.LockListByInitiator[initiator] = new LockIdList();
            }

            var currentList = State.LockListByInitiator[initiator];
            currentList.Ids.Add(lockId);
            State.LockListByInitiator[initiator] = currentList;
        }

        private void UpdateLockReceiversIds(Address receiver, long lockId)
        {
            if (State.LockListForReceiver[receiver] == null)
            {
                State.LockListForReceiver[receiver] = new LockIdList();
            }

            var currentList = State.LockListForReceiver[receiver];
            currentList.Ids.Add(lockId);
            State.LockListForReceiver[receiver] = currentList;
        }

        private void UpdateLockAssetIds(string tokenSymbol, long lockId)
        {
            if (State.LockAssetIdList[tokenSymbol] == null)
            {
                State.LockAssetIdList[tokenSymbol] = new LockIdList();
            }

            var currentList = State.LockAssetIdList[tokenSymbol];
            currentList.Ids.Add(lockId);
            State.LockAssetIdList[tokenSymbol] = currentList;
        }

        private void IncrementAssetLockedCounter(string tokenSymbol, long amount)
        {
            if (State.AssetCounter[tokenSymbol] == null)
            {
                State.AssetCounter[tokenSymbol] = new LockAssetCounter();
            }

            var assetCounter = State.AssetCounter[tokenSymbol];
            assetCounter.LockedAmount += amount;
            State.AssetCounter[tokenSymbol] = assetCounter;

            var assetCounterListObj = State.AssetCounterList.Value;

            if (!assetCounterListObj.TokenSymbols.Any(x => x == tokenSymbol))
            {
                assetCounterListObj.TokenSymbols.Add(tokenSymbol);
                State.AssetCounterList.Value = assetCounterListObj;
            }
        }

        private void IncrementAssetUnlockedCounter(string tokenSymbol, long amount)
        {
            if (State.AssetCounter[tokenSymbol] == null)
            {
                State.AssetCounter[tokenSymbol] = new LockAssetCounter();
            }

            var assetCounter = State.AssetCounter[tokenSymbol];
            assetCounter.UnlockedAmount += amount;
            State.AssetCounter[tokenSymbol] = assetCounter;
        }
    }
}