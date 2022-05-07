using AElf.Contracts.MultiToken;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
    public partial class MultiCrowdSaleContract
    {
        private void AssertContractHasBeenInitialized()
        {
            Assert(State.Admin.Value != null, "Contract is not yet initialized");
        }

        private void AssertSenderIsAdmin()
        {
            Assert(Context.Sender == State.Admin.Value, "Sender should be admin.");
        }

        private void AssertSymbolIsExists(string symbol)
        {
            var tokenInfo = State.TokenContract.GetTokenInfo.Call(new GetTokenInfoInput()
            {
                Symbol = symbol
            });

            Assert(tokenInfo != null && !string.IsNullOrEmpty(tokenInfo.Symbol), "Token doesn't exists.");
        }

        private void AssertSymbolIssuerAndCrowdSaleInitiatorMustBeTheSame(string symbol, Address creator)
        {
            var tokenInfo = State.TokenContract.GetTokenInfo.Call(new GetTokenInfoInput()
            {
                Symbol = symbol
            });

            Assert(tokenInfo != null && !string.IsNullOrEmpty(tokenInfo.Symbol), "Token doesn't exists.");
            Assert(tokenInfo.Issuer == creator, "Only the issuer (creator) of the token can create a crowd sale.");
        }

        private TokenInfo GetNativeToken()
        {
            return State.TokenContract.GetNativeTokenInfo.Call(new Empty()); 
        }

        private long GetChainAmount(long value, int decimals)
        {
            long result = 1;

            if (decimals > 0)
            {
                for (int i = 1; i <= decimals; ++i)
                {
                    result *= value;
                }
            }
            else if (decimals < 0)
            {
                for (int i = -1; i >= decimals; --i)
                {
                    result /= value;
                }
            }

            return result;
        }
    }
}
