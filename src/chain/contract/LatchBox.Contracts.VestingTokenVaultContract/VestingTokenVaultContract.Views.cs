using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace LatchBox.Contracts.VestingTokenVaultContract
{
    public partial class VestingTokenVaultContract
    {
        public override Int64Value GetVestingsCount(Empty input)
        {
            return new Int64Value { Value = State.SelfIncresingVestingId.Value - 1 };
        }

        public override GetVestingTransactionOutput GetVestingTransaction(Int64Value input)
        {
            var vestingId = input.Value;

            Assert(vestingId < State.SelfIncresingVestingId.Value, "Vesting doesn't exists.");

            var output = new GetVestingTransactionOutput();

            var vestingObj = State.Vestings[vestingId];

            output.Vesting = vestingObj;

            var periodIds = State.VestingPeriodList[vestingObj.VestingId].PeriodIds;

            foreach (var periodId in periodIds)
            {
                var periodObj = State.VestingPeriods[vestingObj.VestingId][periodId];

                var period = new VestingTransactionPeriodOutput()
                {
                    Period = periodObj
                };

                var receiverAddresses = State.VestingReceiverList[periodId].Receivers;
                foreach (var receiverAddress in receiverAddresses)
                {
                    var receiver = State.VestingReceivers[periodId][receiverAddress];
                    period.Receivers.Add(receiver);
                }

                output.Periods.Add(period);
            }

            return output;
        }

        public override GetVestingListOutput GetVestingsByInitiator(Address input)
        {
            var output = new GetVestingListOutput();

            var initiator = input;
            var vestingIdList = State.VestingListByInitiator[initiator];

            if (vestingIdList != null)
            {
                foreach (var vestingId in vestingIdList.Ids)
                {
                    var vestingObj = State.Vestings[vestingId];

                    var transactionObj = new GetVestingOutput()
                    {
                        Vesting = vestingObj
                    };

                    var periodIds = State.VestingPeriodList[vestingObj.VestingId].PeriodIds;

                    foreach (var periodId in periodIds)
                    {
                        var periodObj = State.VestingPeriods[vestingObj.VestingId][periodId];
                        transactionObj.Periods.Add(periodObj);
                    }

                    output.Transactions.Add(transactionObj);
                }
            }

            return output;
        }

        public override GetVestingReceiverListOutput GetVestingsForReceiver(Address input)
        {
            var output = new GetVestingReceiverListOutput();
            var receiver = input;

            var vestingPeriodList = State.VestingListForReceivers[input];

            if (vestingPeriodList != null)
            {
                foreach (var vestingPeriod in vestingPeriodList.List)
                {
                    var vestingObj = State.Vestings[vestingPeriod.VestingId];
                    var periodObj = State.VestingPeriods[vestingPeriod.VestingId][vestingPeriod.PeriodId];
                    var receiverObj = State.VestingReceivers[vestingPeriod.PeriodId][receiver];

                    var transactionObj = new GetVestingTransactionForReceiverOutput()
                    {
                        Vesting = vestingObj,
                        Period = periodObj,
                        Receiver = receiverObj
                    };

                    output.Transactions.Add(transactionObj);
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
    }
}
