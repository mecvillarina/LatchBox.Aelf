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

            Assert(tokenInfo != null && !string.IsNullOrEmpty(tokenInfo.Symbol), "Token is not exists.");
        }

        private void AssertSymbolIssuerAndCrowdSaleInitiatorMustBeTheSame(string symbol, Address creator)
        {
            var tokenInfo = State.TokenContract.GetTokenInfo.Call(new GetTokenInfoInput()
            {
                Symbol = symbol
            });

            Assert(tokenInfo != null && !string.IsNullOrEmpty(tokenInfo.Symbol), "Token is not exists.");
            Assert(tokenInfo.Issuer == creator, "Token Issuer and Crowd Sale Creator MUST be the same.");
        }

        private TokenInfo GetNativeToken()
        {
            return State.TokenContract.GetNativeTokenInfo.Call(new Empty()); 
        }
    }
}
