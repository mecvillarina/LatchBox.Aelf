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

            Assert(tokenInfo != null && !string.IsNullOrEmpty(tokenInfo.Symbol), "Token not found.");
        }

        private void AssertSymbolIssuerAndCrowdSaleInitiatorMustBeTheSame(string symbol, Address creator)
        {
            var tokenInfo = GetTokenInfo(symbol);
            
            Assert(tokenInfo != null && !string.IsNullOrEmpty(tokenInfo.Symbol), "Token not found.");
            Assert(tokenInfo.Issuer == creator, "Only the issuer (creator) of the token can create a crowd sale.");
        }

        private TokenInfo GetNativeToken()
        {
            return State.TokenContract.GetNativeTokenInfo.Call(new Empty()); 
        }

        private TokenInfo GetTokenInfo(string symbol)
        {
            return State.TokenContract.GetTokenInfo.Call(new GetTokenInfoInput() { Symbol = symbol });
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

        private void RemoveFromActiveCrowdSales(long crowdSaleId)
        {
            var activeCrowdSaleIds = State.ActiveCrowdSales.Value;

            if (activeCrowdSaleIds.Ids.Contains(crowdSaleId))
            {
                activeCrowdSaleIds.Ids.Remove(crowdSaleId);
                State.ActiveCrowdSales.Value = activeCrowdSaleIds;
            }
        }

        private CrowdSaleOutput GetCrowdSaleOutput(long crowdSaleId)
        {
            var crowdSale = State.CrowdSales[crowdSaleId];
            var tokenInfo = GetTokenInfo(crowdSale.TokenSymbol);
            return new CrowdSaleOutput()
            {
                CrowdSale = crowdSale,
                RaisedAmount = State.CrowdSaleRaiseAmounts[crowdSaleId],
                TokenName = tokenInfo.TokenName,
                TokenDecimals = tokenInfo.Decimals,
                TokenSymbol = tokenInfo.Symbol
            };
        }

    }
}
