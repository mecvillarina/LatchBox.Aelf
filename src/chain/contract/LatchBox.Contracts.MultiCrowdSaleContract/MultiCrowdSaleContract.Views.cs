using AElf.Types;

namespace LatchBox.Contracts.MultiCrowdSaleContract
{
    public partial class MultiCrowdSaleContract
    {
        public override CrowdSaleOutput GetCrowdSale(GetCrowdSaleInput input)
        {
            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Sale Id");

            return GetCrowdSaleOutput(input.CrowdSaleId);
        }

        public override CrowdSaleListOutput GetCrowdSalesByInitiator(Address input)
        {
            var crowdSalesByInitiator = State.CrowdSalesByInitiator[input] ?? new CrowdSaleIds();

            var output = new CrowdSaleListOutput();

            foreach (var crowdSaleId in crowdSalesByInitiator.Ids)
            {
                output.List.Add(GetCrowdSaleOutput(crowdSaleId));
            }

            return output;
        }

        public override CrowdSaleListOutput GetCrowdSales(GetCrowdSalesInput input)
        {
            var output = new CrowdSaleListOutput();

            if (!input.IsUpcoming && !input.IsOngoing)
            {
                for (int i = 1; i < State.SelfIncresingCrowdSaleId.Value; i++)
                {
                    var sale = GetCrowdSaleOutput(i);
                    output.List.Add(sale);
                }
            }
            else
            {
                var crowdSaleIds = State.ActiveCrowdSales.Value.Ids;

                foreach (var crowdSaleId in crowdSaleIds)
                {
                    var sale = GetCrowdSaleOutput(crowdSaleId);
                    if (input.IsUpcoming && sale.CrowdSale.SaleStartDate >= Context.CurrentBlockTime)
                    {
                        output.List.Add(sale);
                    }
                    else if (input.IsOngoing && sale.CrowdSale.SaleStartDate < Context.CurrentBlockTime && sale.CrowdSale.SaleEndDate > Context.CurrentBlockTime)
                    {
                        output.List.Add(sale);
                    }
                }
            }

            return output;
        }

        public override CrowdSaleInvestmentListOutput GetCrowdSaleInvestments(GetCrowdSaleInvestorsInput input)
        {
            Assert(input.CrowdSaleId < State.SelfIncresingCrowdSaleId.Value, "Invalid Sale Id");

            var crowdSaleId = input.CrowdSaleId;
            var output = new CrowdSaleInvestmentListOutput();

            var investors = State.CrowdSaleInvestors[input.CrowdSaleId].Investors;

            foreach (var investor in investors)
            {
                var investment = State.CrowdSaleInvestments[crowdSaleId][investor];
                output.List.Add(investment);
            }

            return output;
        }
    }
}
