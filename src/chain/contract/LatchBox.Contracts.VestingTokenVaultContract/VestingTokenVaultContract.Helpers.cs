using AElf.Contracts.MultiToken;
using Google.Protobuf.Collections;

namespace LatchBox.Contracts.VestingTokenVaultContract
{
    public partial class VestingTokenVaultContract
    {
        private void AssertContractHasBeenInitialized()
        {
            Assert(State.Admin.Value != null, "Contract is not yet initialized");
        }

        private void AssertSenderIsAdmin()
        {
            Assert(Context.Sender == State.Admin.Value, "Sender should be admin.");
        }

        private void AssertSymbolExists(string symbol)
        {
            var tokenInfo = State.TokenContract.GetTokenInfo.Call(new GetTokenInfoInput()
            {
                Symbol = symbol
            });

            Assert(tokenInfo != null && !string.IsNullOrEmpty(tokenInfo.Symbol), "Token not found.");
        }

        private void AssertValidVestingPeriodInputData(RepeatedField<AddVestingPeriodInput> periods, long totalAmount)
        {
            Assert(periods != null && periods.Count > 1, "Vesting MUST have at least 2 periods.");

            long vestingTotalAmountCheck = 0;

            foreach (var period in periods)
            {
                long periodTotalAmountCheck = 0;

                Assert(Context.CurrentBlockTime.ToDateTime() < period.UnlockTime.ToDateTime(), "The parameter date unlock MUST be future date.");

                Assert(period.Receivers != null && period.Receivers.Count > 0, "Every period MUST have at least one receiver.");

                foreach (var receiver in period.Receivers)
                {
                    Assert(receiver.Amount > 0, "All receiver amount must be greater than 0.");
                    periodTotalAmountCheck += receiver.Amount;
                    vestingTotalAmountCheck += receiver.Amount;
                }

                Assert(periodTotalAmountCheck == period.TotalAmount, "The period total amount is not equal to the summation receivers' amounts on that period.");
            }

            Assert(vestingTotalAmountCheck == totalAmount, "The total amount is not equal to the summation receivers' amounts.");
        }
    }
}
