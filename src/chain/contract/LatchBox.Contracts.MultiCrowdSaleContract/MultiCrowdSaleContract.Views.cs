using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
    public partial class MultiCrowdSaleContract
    {
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

        public override CrowdSaleListOutput GetCrowdSalesByInitiator(Address input)
        {
            var crowdSalesByInitiator = State.CrowdSalesByInitiator[input] ?? new CrowdSaleIds();

            var output = new CrowdSaleListOutput();

            foreach (var crowdSaleId in crowdSalesByInitiator.Ids)
            {
                output.CrowdSales.Add(GetCrowdSaleOutput(crowdSaleId));
            }

            return output;
        }

        public override CrowdSaleListOutput GetActiveCrowdSales(Empty input)
        {
            var crowdSaleIds = State.ActiveCrowdSales.Value.Ids;

            var output = new CrowdSaleListOutput();

            foreach (var crowdSaleId in crowdSaleIds)
            {
                output.CrowdSales.Add(GetCrowdSaleOutput(crowdSaleId));
            }

            return output;
        }
    }
}
