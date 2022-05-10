using AElf.Contracts.MultiToken;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace LatchBox.Contracts.LockTokenVaultContract
{
    public partial class LockTokenVaultContract
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

        private void AssertValidLockReceiverInputData(RepeatedField<AddLockReceiverInput> receivers, long totalAmount)
        {
            Assert(receivers != null && receivers.Count > 0, "Lock MUST have at least one receiver.");

            long totalAmountCheck = 0;
            foreach (var receiver in receivers)
            {
                Assert(receiver.Amount > 0, "All receiver amount must be greater than 0.");
                totalAmountCheck += receiver.Amount;
            }

            Assert(totalAmountCheck == totalAmount, "The total amount is not equal to the summation receivers' amounts.");
        }
    }
}
