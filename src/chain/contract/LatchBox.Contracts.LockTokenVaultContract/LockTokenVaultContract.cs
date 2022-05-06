using AElf.Sdk.CSharp;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace LatchBox.Contracts.LockTokenVaultContract
{
    /// <summary>
    /// The C# implementation of the contract defined in lock_token_vault_contract.proto that is located in the "protobuf"
    /// folder.
    /// Notice that it inherits from the protobuf generated code. 
    /// </summary>
    public partial class LockTokenVaultContract : LockTokenVaultContractContainer.LockTokenVaultContractBase
    {
        public override Empty Initialize(InitializeInput input)
        {
            Assert(State.Admin.Value == null, "Already initialized");

            State.Admin.Value = Context.Sender;

            State.TokenContract.Value = Context.GetContractAddressByName(SmartContractConstants.TokenContractSystemName);
            State.ConsensusContract.Value = Context.GetContractAddressByName(SmartContractConstants.ConsensusContractSystemName);

            State.LockIndex.Value = 0;

            return new Empty();
        }

        public override Empty UpdateTokenVaultSettings(UpdateTokenVaultSettingsInput input)
        {
            AssertContractHasBeenInitialized();
            AssertSenderIsAdmin();

            State.TokenVaultSettings.Value = new TokenVaultSettings();
            State.TokenVaultSettings.Value.FoundationAddress = input.FoundationAddress;
            State.TokenVaultSettings.Value.PlatformAddress = input.PlatformAddress;
            State.TokenVaultSettings.Value.StakingAddress = input.StakingAddress;

            return new Empty();
        }

        public override Empty UpdatePayment(UpdatePaymentInput input)
        {
            AssertContractHasBeenInitialized();
            AssertSenderIsAdmin();

            State.Payment.Value = new LatchBoxPayment();
            State.Payment.Value.TokenSymbol = input.TokenSymbol;
            State.Payment.Value.AddLockFee = input.AddLockPaymentFee;
            State.Payment.Value.ClaimLockFee = input.ClaimLockPaymentFee;
            State.Payment.Value.RevokeLockFee = input.RevokeLockPaymentFee;

            return new Empty();
        }

        public override Empty AddLock(AddLockInput input)
        {
            var currentBlockTime = Context.CurrentBlockTime;

            AssertContractHasBeenInitialized();
            Assert(input.TotalAmount > 0, "The input Total Amount MUST be greater than 0.");
            Assert(input.UnlockTime.ToDateTime().DayOfYear >= currentBlockTime.ToDateTime().DayOfYear, "The input Unlock Time MUST be at least 1 day.");
            ValidateLockReceiverInputData(input.Receivers, input.TotalAmount);

            //Add Token Transfers here.

            var lockIdx = State.LockIndex.Value;
            State.LockIndex.Value = lockIdx + 1;

            var initiator = Context.Sender;

            var lbLock = new LatchBoxLock
            {
                TokenSymbol = input.TokenSymbol,
                InitiatorAddress = initiator,
                CreationTime = currentBlockTime,
                StartTime = currentBlockTime,
                UnlockTime = input.UnlockTime,
                IsActive = true,
                IsRevocable = input.IsRevocable,
                IsRevoked = false
            };

            foreach (var receiverInput in input.Receivers)
            {
                LatchBoxLockReceiver receiver = new LatchBoxLockReceiver
                {
                    ReceiverAddress = receiverInput.ReceiverAddress,
                    Amount = receiverInput.Amount,
                    DateClaimed = null,
                    DateRevoked = null,
                    IsActive = true
                };
                lbLock.Receivers.Add(receiver);
            }

            State.Locks[lockIdx] = lbLock;

            UpdateLockInitiatorIndexes(initiator, lockIdx);

            foreach (var receiver in lbLock.Receivers)
            {
                UpdateLockReceiversIndexes(receiver.ReceiverAddress, lockIdx);
            }

            UpdateLockAssetIndexes(input.TokenSymbol, lockIdx);
            IncrementAssetLockedCounter(input.TokenSymbol, input.TotalAmount);
            Context.Fire(new OnCreatedLockEvent
            {
                LockIdx = lockIdx
            });

            return new Empty();
        }

        private void UpdateLockInitiatorIndexes(Address initiator, ulong lockIdx)
        {
            if (State.LockInitiatorIndexes[initiator] == null)
            {
                State.LockInitiatorIndexes[initiator] = new LockIndexList();
            }

            var currentIndexes = State.LockInitiatorIndexes[initiator];
            currentIndexes.Indexes.Add(lockIdx);
            State.LockInitiatorIndexes[initiator] = currentIndexes;
        }

        private void UpdateLockReceiversIndexes(Address receiver, ulong lockIdx)
        {
            if (State.LockReceiverIndexes[receiver] == null)
            {
                State.LockReceiverIndexes[receiver] = new LockIndexList();
            }

            var currentIndexes = State.LockReceiverIndexes[receiver];
            currentIndexes.Indexes.Add(lockIdx);
            State.LockReceiverIndexes[receiver] = currentIndexes;
        }

        private void UpdateLockAssetIndexes(string tokenSymbol, ulong lockIdx)
        {
            if (State.LockAssetIndexes[tokenSymbol] == null)
            {
                State.LockAssetIndexes[tokenSymbol] = new LockIndexList();
            }

            var currentIndexes = State.LockAssetIndexes[tokenSymbol];
            currentIndexes.Indexes.Add(lockIdx);
            State.LockAssetIndexes[tokenSymbol] = currentIndexes;
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