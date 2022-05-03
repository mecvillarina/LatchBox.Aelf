using Google.Protobuf.WellKnownTypes;

namespace LatchBox.Contracts.LockTokenVaultContract
{
    public partial class LockTokenVaultContract
    {
        public override UInt64Value GetLocksCount(Empty input)
        {
            return new UInt64Value { Value = State.LockIndex.Value };
        }

        public override GetLatchBoxLockTransactionOutput GetLockTransaction(UInt64Value input)
        {
            var lockIndex = input.Value;

            Assert(lockIndex < State.LockIndex.Value, "LatchBox Lock doesn't exists.");

            var lbLock = State.Locks[input.Value];

            return new GetLatchBoxLockTransactionOutput() { Index = lockIndex, Lock = lbLock };
        }
    }
}
