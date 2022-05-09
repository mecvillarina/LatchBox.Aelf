using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace LatchBox.Contracts.LockTokenVaultContract
{
    public partial class LockTokenVaultContract
    {
        public override Int64Value GetLocksCount(Empty input)
        {
            return new Int64Value { Value = State.SelfIncresingLockId.Value - 1 };
        }

        public override GetLockTransactionOutput GetLockTransaction(Int64Value input)
        {
            var lockId = input.Value;

            Assert(lockId < State.SelfIncresingLockId.Value, "Lock doesn't exists.");

            var output = new GetLockTransactionOutput();

            var lockObj = State.Locks[input.Value];
            var receiverAddresses = State.LockReceiverList[lockObj.LockId].Receivers;

            output.Lock = lockObj;
            foreach (var receiverAddress in receiverAddresses)
            {
                var receiverObj = State.LockReceivers[lockObj.LockId][receiverAddress];
                output.Receivers.Add(receiverObj);
            }

            return output;
        }

        public override GetLockListOutput GetLocks(Empty input)
        {
            var output = new GetLockListOutput();

            for (int i = 1; i < State.SelfIncresingLockId.Value; i++)
            {
                var lockObj = State.Locks[i];
                output.Locks.Add(lockObj);
            }

            return output;
        }

        public override GetLockListOutput GetLocksByInitiator(Address input)
        {
            var output = new GetLockListOutput();

            var lockIdList = State.LockListByInitiator[input];

            if (lockIdList != null)
            {
                foreach (var lockId in lockIdList.Ids)
                {
                    var lockObj = State.Locks[lockId];
                    output.Locks.Add(lockObj);
                }
            }

            return output;
        }

        public override GetLockReceiverListOutput GetLocksForReceiver(Address input)
        {
            var output = new GetLockReceiverListOutput();

            var lockIdList = State.LockListForReceiver[input];

            if (lockIdList != null)
            {
                foreach (var lockId in lockIdList.Ids)
                {
                    var itemOutput = new GetLockTransactionForReceiverOutput();
                    var lockObj = State.Locks[lockId];
                    itemOutput.Lock = lockObj;
                    itemOutput.Receiver = State.LockReceivers[lockId][input];
                    output.LockTransactions.Add(itemOutput);
                }
            }

            return output;
        }

        public override GetLockListOutput GetLocksByAsset(StringValue input)
        {
            var tokenSymbol = input.Value;

            AssertSymbolExists(tokenSymbol);

            var output = new GetLockListOutput();

            var lockIdList = State.LockAssetIdList[tokenSymbol];

            if (lockIdList != null)
            {
                foreach (var lockId in lockIdList.Ids)
                {
                    var lockObj = State.Locks[lockId];
                    output.Locks.Add(lockObj);
                }
            }

            return output;
        }

        public override GetRefundListOutput GetRefunds(Empty input)
        {
            var output = new GetRefundListOutput();

            var refundsObj = State.Refunds[Context.Sender];

            if (refundsObj != null)
            {
                var refunds = refundsObj.Refunds;
                output.Refunds.AddRange(refunds);
            }

            return output;
        }

        public override GetLockAssetCounterListOutput GetAssetsCounter(Empty input)
        {
            var output = new GetLockAssetCounterListOutput();

            foreach (var tokenSymbol in State.AssetCounterList.Value.TokenSymbols)
            {
                var assetCounter = State.AssetCounter[tokenSymbol];
                output.Assets.Add(new GetLockAssetCounterOutput()
                {
                    TokenSymbol = tokenSymbol,
                    LockedAmount = assetCounter.LockedAmount,
                    UnlockedAmount = assetCounter.UnlockedAmount
                });
            }

            return output;
        }
    }
}
